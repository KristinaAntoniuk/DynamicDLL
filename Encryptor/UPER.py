import asn1tools
import json

class UPER:
    
    def __init__(self, schema):
        self.schema = asn1tools.compile_string(schema, 'uper')

    def encode(self, name, data):
        data = json.loads(data)
        self.encoded = self.schema.encode(name, data)
        return self.encoded
        
    def decode(self, name, data):
        data = data.encode('windows-1252')
        return self.schema.decode(name, data)