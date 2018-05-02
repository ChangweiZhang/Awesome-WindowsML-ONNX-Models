from coremltools.models.utils import load_spec
from winmltools import convert_coreml
from winmltools.utils import save_model
from winmltools.utils import save_text
import os

print 'CoreML model path:'

ml_file = input()

ml_path = os.path.split(ml_file)

folder_path = ml_path[0]

# Load model file
model_coreml = load_spec(ml_file)

print 'ONNX model name:'
ml_name = input()
# Convert it!
# The automatic code generator (mlgen) uses the name parameter to generate
# class names.
model_onnx = convert_coreml(model_coreml, name=ml_name)

onnx_file_name = folder_path +'\\' + ml_path[1].split('.')[0]

# Save the produced ONNX model in binary format
onnx_path=onnx_file_name + '.onnx'

print onnx_path 

save_model(model_onnx,onnx_path)


#save_text(model_onnx, 'example.txt')
# Save as text
save_text(model_onnx, onnx_file_name + '.json')