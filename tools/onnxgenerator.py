import os
from winmltools import convert_coreml
from winmltools.utils import load_model,save_model, save_text, convert_float_to_float16
from coremltools.models.utils import load_spec
import winmltools

print('CoreML/ONNX model path:')
ml_file = str(input())
ml_path = os.path.split(ml_file)
folder_path = ml_path[0]
format=ml_path[-1].split('.')[-1]
if format=='onnx':
    model_onnx=load_model(ml_file)
else:
    # Load model file
    model_coreml = load_spec(ml_file)

    print('ONNX model name:')
    ml_name = str(input())

# Convert it!
# The automatic code generator (mlgen) uses the name parameter to generate
# class names.

    model_onnx = convert_coreml(model_coreml, name=ml_name)
    
onnx_file_name = os.path.join(folder_path, ml_path[1].split('.')[0])

print('Convert to floating point 16? (y/n)')
need_float_16=input()
if(need_float_16=='y'):
    model_onnx=convert_float_to_float16(model_onnx)

# Save the produced ONNX model in binary format
onnx_path = onnx_file_name+'16bit.onnx'
save_model(model_onnx, onnx_path)
print(onnx_path)

#save_text(model_onnx, 'example.txt')
# Save as text
print('do you want generate json model file? (y/n)')
need_json = str(input())
if(need_json == 'y'):
    json_path = onnx_file_name + '.json'
    save_text(model_onnx, json_path)
    print(json_path)
