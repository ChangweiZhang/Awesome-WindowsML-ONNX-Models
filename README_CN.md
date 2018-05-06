# Awesome WindowsML ONNX Models

从Windows 10 RS4更新（版本1803）开始，微软提供了系统内置的AI平台让开发者可以直接将机器学习能力集成到应用中，可以在本地离线执行与训练好的机器学习模型。

[官方文档入口](https://docs.microsoft.com/en-us/windows/uwp/machine-learning/)

我们可以使用大量ONNX格式的机器学习模型，在Windows ML平台下利用机器学习技术在本机实现创新体验。本项目提供目前尽可能多的经过验证的模型，并且提供相应的demo和引用信息，帮助大家快速集成AI能力到项目。

同时还提供ONNX模型转换工具，可以将其他格式的模型转化为可使用ONNX格式。




## 模型集合
  
目前项目的模型种类持续扩充中，***master***分支仅提供经过验证测试的模型和demo。

### 图像处理
  
图像处理类模型可以根据输入的图像计算出特定的结果输出


| 模型名称  | 功能  |  来源| |
|:------------- |:---------------:| :-------------:|:---------:|
| GoogleNetPlace      | 识别图像的场景类型，输出205种类别，例如办公室、机场之类 |[CoreML](https://coreml.store/googlenetplaces)|[模型下载](http://changwei.tech/doc/onnx) [Demo](https://github.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/tree/master/src/WindowsML-Demos/GoogleNetPlaces) [参考文献](http://places.csail.mit.edu/index.html) |
| Inception v3      | 识别图像中的物体，输出1000种类别。前5个预测错误低至5.6%        | [CoreML](https://coreml.store/inceptionv3)|          [模型下载](http://changwei.tech/doc/onnx) [Demo](https://github.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/tree/master/src/WindowsML-Demos/InceptionV3) [参考文献](https://arxiv.org/abs/1512.00567) |
| ResNet50 | 识别图像中的物体，输出1000种类别。前5个预测错误低至7.8%        |   [CoreML](https://coreml.store/resnet50)|       [模型下载](http://changwei.tech/doc/onnx) [Demo](https://github.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/tree/master/src/WindowsML-Demos/ResNet50) [参考文献](https://arxiv.org/abs/1512.03385)  |
| TinyYOLO | 识别出图像中多个物体，并输出类别和物体矩形边框数据，用于在图像中圈出物体，可识别物体种类为20种       | [CoreML](https://coreml.store/tinyyolo)|        [模型下载](http://changwei.tech/doc/onnx) [Demo](https://github.com/ChangweiZhang/Awesome-WindowsML-ONNX-Models/tree/master/src/WindowsML-Demos/TinyYOLO) [参考文献](http://machinethink.net/blog/object-detection-with-yolo) |
  
  
## ONNX转换工具

ONNX是由微软、Facebook和英特尔等公司推出的一个通用开放的机器学习模型格式，Windows ML目前只能执行ONNX格式的模型。所以我们需要将其他格式的模型转换后才可以使用，项目给大家提供了一个快速转换工具ONNX Generator。

[ONNX项目地址](https://github.com/onnx/onnx)

[微软官方模型转换文档](https://docs.microsoft.com/en-us/windows/uwp/machine-learning/conversion-samples)

###环境准备

请先安装好以下工具：

* Python 2.7.x
* winmltools
* [coremltools](https://github.com/apple/coremltools)


### 工具使用

ONNX Generator工具位于tools目录下，直接执行onnxgenerator.py脚本即可：


```
python onnxgenerator.py
```

根据输出窗口提示，依次输入CoreML模型文件地址并回车：

![MacDown logo](http://macdown.uranusjr.com/static/images/logo-160.png)

现在输入模型名称，用于MLGen工具生成C#代码的模型命名

![MacDown logo](http://macdown.uranusjr.com/static/images/logo-160.png)

生成ONNX模型后，此时决定是否继续生成json格式的模型文件

![MacDown logo](http://macdown.uranusjr.com/static/images/logo-160.png)

生成的模型文件如下：

![MacDown logo](http://macdown.uranusjr.com/static/images/logo-160.png)

## Demo说明

src目录中的demo项目中默认是***没有模型文件的***，此时直接编译会报错失败。

***请直接把下载后的ONNX模型文件放入项目中对应的位置即可***

## 问题反馈

如果有问题可以直接在Issue中提出，或者联系我本人。

联系方式：


* E-mail: [mantgh@outlook.com](mailto://mantgh@outlook.com)
* Weibo: @msp的昌伟哥哥
