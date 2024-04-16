import string
from tokenize import String
from flask import Flask, request
from UPER import UPER

app = Flask(__name__)

@app.route('/encode', methods=['GET'])
def encode():
    schema = request.args.get('schema')
    name = request.args.get('name')
    data = request.get_data()
    uperClass = UPER(schema);
    return uperClass.encode(name, data)

@app.route('/decode', methods=['GET'])
def decode():
    schema = request.args.get('schema')
    name = request.args.get('name')
    data = request.get_data()
    uperClass = UPER(schema);
    return uperClass.decode(name, data)
    
app.run()