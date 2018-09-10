# Awesome WindowsML ONNX Models
[Guidelines in Chinese（中文版指南）](https://github.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/blob/master/README_CN.md)

Since Windows 10 RS4 Update（Build 1803），Microsoft has released Windows ML platform to help developers integrate machine learning features into applications. 

[The official documentation](https://docs.microsoft.com/en-us/windows/uwp/machine-learning/)

We can create innovative apps based on lots of machine learning models in ONNX format because Windows ML evaluates trained models locally on Windows 10 devices, providing hardware-accelerated performance by leveraging the device's CPU or GPU, and computes evaluations for both classical ML algorithms and Deep Learning.
This project provides the largest collection of tested ONNX machine learning models and demos for developers，to help you integrate machine learning features more easily.

What's more, it also provide a ONNX model generator that is able to convert CoreML models to ONNX format.
## Requirements

- [Visual Studio 2017 Version 15.7.4 or Newer](https://developer.microsoft.com/en-us/windows/downloads)
- [Windows 10 - Build 17738 or higher](https://www.microsoft.com/en-us/software-download/windowsinsiderpreviewiso)
- [Windows SDK - Build 17738 or higher](https://www.microsoft.com/en-us/software-download/windowsinsiderpreviewSDK)

## Models
  
The master branch only provide tested models for you.

### Image Processing
  
Models that can output deseried information with image data as input .



| Name  | Feature  |  Source| |
|:------------- |:---------------:| :-------------:|:---------:|
| GoogleNetPlace      | Detects the scene of an image from 205 categories such as airport, bedroom, forest, coast etc. |[CoreML](https://coreml.store/googlenetplaces)|[Download](https://github.com/ChangweiZhang/Awesome-ONNX-Models) [Demo](https://github.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/tree/master/src/WindowsML-Demos/GoogleNetPlaces) [Reference](http://places.csail.mit.edu/index.html) |
| Inception v3      | Detects the dominant objects present in an image from a set of 1000 categories such as trees, animals, food, vehicles, person etc. The top-5 error from the original publication is 5.6%.        | [CoreML](https://coreml.store/inceptionv3)|          [Downloads](https://github.com/ChangweiZhang/Awesome-ONNX-Models) [Demo](https://github.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/tree/master/src/WindowsML-Demos/InceptionV3) [Reference](https://arxiv.org/abs/1512.00567) |
| ResNet50 | Detects the dominant objects present in an image from a set of 1000 categories such as trees, animals, food, vehicles, person etc. The top-5 error from the original publication is 7.8%.        |   [CoreML](https://coreml.store/resnet50)|       [Download](https://github.com/ChangweiZhang/Awesome-ONNX-Models) [Demo](https://github.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/tree/master/src/WindowsML-Demos/ResNet50) [Reference](https://arxiv.org/abs/1512.03385)  |
| TinyYOLO | Detects multi objects in an image. The Tiny YOLO network from the paper \'YOLO9000: Better, Faster, Stronger\' (2016), arXiv:1612.08242       | [CoreML](https://coreml.store/tinyyolo)|        [Download](https://github.com/ChangweiZhang/Awesome-ONNX-Models) [Demo](https://github.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/tree/master/src/WindowsML-Demos/TinyYOLO) [Reference](http://machinethink.net/blog/object-detection-with-yolo) |
| LocationNet | Predict the location where a picture was taken. |[CoreML](https://coreml.store/rn1015k500)|[Download](https://cwonnxstorage.blob.core.windows.net/onnx/RN1015k500.onnx) [Demo](https://github.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/tree/master/src/WindowsML-Demos/LocationNet) [Reference](https://aws.amazon.com/blogs/ai/estimating-the-location-of-images-using-mxnet-and-multimedia-commons-dataset-on-aws-ec2) |
| Fast Netural Style Transfer      | Transfer a image into various artist styles,including Candy, Feathers, La Muse, Mosica, Scream, Udnie. https://github.com/jcjohnson/fast-neural-style |[CoreML](https://coreml.store/googlenetplaces)|[Download](https://github.com/ChangweiZhang/Awesome-ONNX-Models) [Demo](https://github.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/tree/master/src/WindowsML-Demos/FNSSeries) [Reference](https://cs.stanford.edu/people/jcjohns/eccv16/) |

  
  
## ONNX Generator

ONNX is a open format to represent deep learning models. With ONNX, AI developers can more easily move models between state-of-the-art tools and choose the combination that is best for them. Windows ML only support ONNX format models. So we must need convert existed models in other format to ONNX models and this ONNX Generator is useful for you.

[ONNX Project](https://github.com/onnx/onnx)

[The offical documents for model converting](https://docs.microsoft.com/en-us/windows/uwp/machine-learning/conversion-samples)

### Requirements

* Python 3.6.x
* winmltools
* [coremltools](https://github.com/apple/coremltools)
* ONNX 1.2.2

#### Install ONNX
```
pip install onnx
```
make sure the verison is 1.2.2

#### Install Apple coremltools
```
pip install git+https://github.com/apple/coremltools
```
#### Install winmltools
```
pip install winmltools
```


### How to Use

ONNX Generator tool is located in the __tools__ folder，and just run the onnxgenerator.py script：


```
python onnxgenerator.py
```

Next step is input the path of CoreML model file：

![model path](https://github.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/blob/master/images/step1.png?raw=true)


Input the name of model, which will be used to generator c# class name by MLGen tool


![model name](https://raw.githubusercontent.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/master/images/step2.png)

Convert to floating point 16

![Optimize to float16](https://raw.githubusercontent.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/master/images/step3.jpg)

After ONNX file generated, you can confirm if you want a json model file


![generate json](https://raw.githubusercontent.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/master/images/step4.jpg)

Here are the model files：

![output model](https://raw.githubusercontent.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/master/images/result.png)

## Demos

Demo projects in the src folder have __no ONNX model files__ by default and it can't be built.

**You just should download the ONNX file from above links and place them into correct folders.**

![LocationNet](https://raw.githubusercontent.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/master/images/demo.png)

## Feedback

If there is any question or issue, please create a new issue or just contact me. By the way, everyone can contribute to this project.
Have a enjoy time!

Contact：


* E-mail: [mantgh@outlook.com](mailto://mantgh@outlook.com)
* Weibo: @msp的昌伟哥哥
