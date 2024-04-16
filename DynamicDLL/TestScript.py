import asn1tools
schema = asn1tools.compile_files('C:/Users/kristina.antoniuk/Documents/Asfinag/TestASNData/Schema.asn')
data = schema.encode('Rocket', {'range': 3483972938740293874029090, 'name': 'Falcon', 'fuel': 'solid'})
print(data)
decoded = schema.decode('Rocket', data)
print(decoded)