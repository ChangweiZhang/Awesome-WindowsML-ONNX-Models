using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.AI.MachineLearning.Preview;
using Windows.Foundation;
using Windows.Media;
using WindowsMLDemos.Common;
using WindowsMLDemos.Common.Helper;
// TinyYOLOModel

namespace TinyYOLO
{
    public sealed class TinyYOLOModelModelInput : IMachineLearningInput
    {
        public VideoFrame image { get; set; }
    }

    public sealed class TinyYOLOModelModelOutput : IMachineLearningOutput
    {
        public const int ML_ARRAY_LENGTH = 125 * 13 * 13;
        public IList<float> grid { get; set; }
        public TinyYOLOModelModelOutput()
        {
            this.grid = new List<float>();
            for (var i = 0; i < ML_ARRAY_LENGTH; i++)
            {
                grid.Add(float.NaN);
            }
        }
    }

    public sealed class TinyYOLOModelModel : IMachineLearningModel
    {
        public List<string> Labels = new List<string> {
          "aeroplane", "bicycle", "bird", "boat", "bottle", "bus", "car", "cat",
          "chair", "cow", "diningtable", "dog", "horse", "motorbike", "person",
          "pottedplant", "sheep", "sofa", "train", "tvmonitor"
        };
        public List<float> Anchors = new List<float> { 1.08f, 1.19f, 3.42f, 4.41f, 6.63f, 11.38f, 9.42f, 5.11f, 16.62f, 10.52f };
        // Tweak these values to get more or fewer predictions.
        float confidenceThreshold = 0.3f;
        float iouThreshold = 0.5f;
        public static int maxBoundingBoxes = 10;
        public LearningModelPreview LearningModel { get; set; }

        public async Task<IMachineLearningOutput> EvaluateAsync(IMachineLearningInput input)
        {
            var modelInput = input as TinyYOLOModelModelInput;
            TinyYOLOModelModelOutput output = new TinyYOLOModelModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(LearningModel);
            binding.Bind("image", modelInput.image);
            binding.Bind("grid", output.grid);
            LearningModelEvaluationResultPreview evalResult = await LearningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }

        public List<Prediction> ComputeBoundingBoxes(IList<float> features)
        {
            if (features.Count == TinyYOLOModelModelOutput.ML_ARRAY_LENGTH)
            {
                var predictions = new List<Prediction>();

                var blockSize = 32f;
                var gridHeight = 13;
                var gridWidth = 13;
                var boxesPerCell = 5;
                var numClasses = 20;
                // The 416x416 image is divided into a 13x13 grid. Each of these grid cells
                // will predict 5 bounding boxes (boxesPerCell). A bounding box consists of
                // five data items: x, y, width, height, and a confidence score. Each grid
                // cell also predicts which class each bounding box belongs to.
                //
                // The "features" array therefore contains (numClasses + 5)*boxesPerCell
                // values for each grid cell, i.e. 125 channels. The total features array
                // contains 125x13x13 elements.

                // NOTE: It turns out that accessing the elements in the multi-array as
                // `features[[channel, cy, cx] as [NSNumber]].floatValue` is kinda slow.
                // It's much faster to use direct memory access to the features.
                var channelStride = 169;
                var yStride = 13;
                var xStride = 1;

                int Offset(int channel, int x, int y)
                {
                    return channel * channelStride + y * yStride + x * xStride;
                }

                for (var cy = 0; cy < gridHeight; cy++)
                {
                    for (var cx = 0; cx < gridWidth; cx++)
                    {
                        for (var b = 0; b < boxesPerCell; b++)
                        {
                            var channel = b * (numClasses + 5);

                            var tx = features[Offset(channel, cx, cy)];
                            var ty = features[Offset(channel + 1, cx, cy)];
                            var tw = features[Offset(channel + 2, cx, cy)];
                            var th = features[Offset(channel + 3, cx, cy)];
                            var tc = features[Offset(channel + 4, cx, cy)];

                            // The predicted tx and ty coordinates are relative to the location
                            // of the grid cell; we use the logistic sigmoid to constrain these
                            // coordinates to the range 0 - 1. Then we add the cell coordinates
                            // (0-12) and multiply by the number of pixels per grid cell (32).
                            // Now x and y represent center of the bounding box in the original
                            // 416x416 image space.
                            var x = ((float)(cx) + MLHelper.Sigmoid(tx)) * blockSize;
                            var y = ((float)(cy) + MLHelper.Sigmoid(ty)) * blockSize;

                            // The size of the bounding box, tw and th, is predicted relative to
                            // the size of an "anchor" box. Here we also transform the width and
                            // height into the original 416x416 image space.
                            var w = MathF.Exp(tw) * Anchors[2 * b] * blockSize;
                            var h = MathF.Exp(th) * Anchors[2 * b + 1] * blockSize;

                            // The confidence value for the bounding box is given by tc. We use
                            // the logistic sigmoid to turn this into a percentage.
                            var confidence = MLHelper.Sigmoid(tc);

                            // Gather the predicted classes for this anchor box and softmax them,
                            // so we can interpret these numbers as percentages.
                            var classes = new float[numClasses];
                            for (var c = 0; c < numClasses; c++)
                            {
                                // The slow way:
                                //classes[c] = features[[channel + 5 + c, cy, cx] as [NSNumber]].floatValue

                                // The fast way:
                                classes[c] = features[Offset(channel + 5 + c, cx, cy)];
                            }

                            classes = MLHelper.Softmax(classes);

                            // Find the index of the class with the largest score.
                            //var bestClassScore = classes.Max();
                            //var detectedClass = 0;
                            //for (var i = 0; i < classes.Length; i++)
                            //{
                            //    if (classes[i] == bestClassScore)
                            //    {
                            //        detectedClass = i;
                            //        break;
                            //    }
                            //}
                            var res = MLHelper.Argmax(classes);
                            var bestClassScore = res.Item2;
                            var detectedClass = res.Item1;

                            // Combine the confidence score for the bounding box, which tells us
                            // how likely it is that there is an object in this box (but not what
                            // kind of object it is), with the largest class prediction, which
                            // tells us what kind of object it detected (but not where).
                            var confidenceInClass = bestClassScore * confidence;

                            // Since we compute 13x13x5 = 845 bounding boxes, we only want to
                            // keep the ones whose combined score is over a certain threshold.
                            if (confidenceInClass > confidenceThreshold)
                            {
                                var rect = new Rect((x - w / 2), (y - h / 2),
                                                  w, h);

                                var prediction = new Prediction(classIndex: detectedClass,
                                                            score: confidenceInClass,
                                                            rect: rect);
                                predictions.Add(prediction);
                            }
                        }
                    }
                }
                // We already filtered out any bounding boxes that have very low scores,
                // but there still may be boxes that overlap too much with others. We'll
                // use "non-maximum suppression" to prune those duplicate bounding boxes.
                return NonMaxSuppression(boxes: predictions, limit: maxBoundingBoxes, threshold: iouThreshold);
            }
            return null;
        }

        private List<Prediction> NonMaxSuppression(List<Prediction> boxes, int limit, float threshold)
        {
            // Do an argsort on the confidence scores, from high to low.
            var sortedIndices = new List<Prediction>(boxes);
            sortedIndices.Sort((a, b) => -a.Score.CompareTo(b.Score));

            var selected = new List<Prediction>();
            var active = new bool[boxes.Count];
            for (var i = 0; i < active.Length; i++)
            {
                active[i] = true;
            }
            var numActive = active.Length;

            // The algorithm is simple: Start with the box that has the highest score.
            // Remove any remaining boxes that overlap it more than the given threshold
            // amount. If there are any boxes left (i.e. these did not overlap with any
            // previous boxes), then repeat this procedure, until no more boxes remain
            // or the limit has been reached.
            for (var i = 0; i < boxes.Count; i++)
            {
                var shouldBreak = false;
                if (active[i])
                {
                    var boxA = sortedIndices[i];
                    selected.Add(boxA);
                    if (selected.Count >= limit) { break; }

                    for (var j = i + 1; j < boxes.Count; j++)
                    {
                        if (active[j])
                        {
                            var boxB = sortedIndices[j];
                            if (IOU(a: boxA.Rect, b: boxB.Rect) > threshold)
                            {
                                active[j] = false;
                                numActive -= 1;
                                if (numActive <= 0)
                                {
                                    shouldBreak = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (shouldBreak)
                {
                    break;
                }
            }

            return selected;
        }

        private float IOU(Rect a, Rect b)
        {
            var areaA = a.Width * a.Height;
            if (areaA <= 0) { return 0; }

            var areaB = b.Width * b.Height;
            if (areaB <= 0) { return 0; }

            var intersectionMinX = Max(a.X, b.X);
            var intersectionMinY = Max(a.Y, b.Y);
            var intersectionMaxX = Min(a.X + a.Width, b.X + b.Width);
            var intersectionMaxY = Min(a.Y + a.Height, b.Y + b.Height);
            var intersectionArea = Max(intersectionMaxY - intersectionMinY, 0) *
                                   Max(intersectionMaxX - intersectionMinX, 0);
            return (float)(intersectionArea / (areaA + areaB - intersectionArea));
        }

        private float Min(double x, double y)
        {
            return (float)(x < y ? x : y);
        }

        private float Max(double x1, double x2)
        {
            return (float)(x1 > x2 ? x1 : x2);
        }
    }

    public struct Prediction
    {
        public int ClassIndex { get; set; }
        public float Score { get; set; }
        public Rect Rect { get; set; }
        public Prediction(int classIndex, float score, Rect rect)
        {
            ClassIndex = classIndex;
            Score = score;
            Rect = rect;
        }
    }
}
