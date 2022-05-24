# Extractor

## Requirements
- C# 7

You may also need to install the following package to make this project work in your IDE:
- `Newtonsoft.Json` (can be installed with NuGet)


# What will this extractor do:
- Compare a ARM for one API inside APIM with another ARM template for same API. e.g. an old copy of ARM with a newer one. 
- Find out which new resources has been add in the 'newer' (second) ARM template. ---> found under folder `newResources`
- Find out if there is any other small differences between any resource in first ARM with the corresponding resource in the second ARM. ---> found under folder `innerDifferences`



[comment]: <> (# TODOs:)

[comment]: <> (- have a check to empty or null values)

[comment]: <> (- need to remove the dependencies in the splited files --> **done**)

[comment]: <> (- add variabls to AI --> **skipped**)

[comment]: <> (- Create a class that return only the not added resources, for schemas and operations &#40;and probably for AI&#41;)

[comment]: <> (- May do a similar idea that extract logic app.)








