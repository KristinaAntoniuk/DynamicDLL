from UPER import UPER


uperClass = UPER('C:/Users/kristina.antoniuk/Documents/Asfinag/TestASNData/Schema.asn')
encoded = uperClass.encode('Rocket', 'C:/Users/kristina.antoniuk/Documents/Asfinag/TestASNData/Data.asn')
decoded = uperClass.decode('Rocket')
print(decoded)

