SET PATH="c:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\"

xsd Grid3d.xml
xsd /c /n:ErnestoKava.Geophysics.Serializers.Grid3d layers.xsd
del layers.xsd