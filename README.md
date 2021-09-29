# Extractor

## Requirements
- C# 7

You may also need to install the following package to make this project work in your IDE:
- Newtonsoft.Json (can be installed with NuGet)


- The template file (Template.json need to be copied to debug). This can be done be changing the properties of the Template.json and then choose 'Copy if newer' option 

# What will this extractor/cleaner do:
- Move the API itself to another file
- Move the diagnostics to another file
- Move the operations and their policies to another file
- Move all other things to another file


- Maybe also extract only the requested operation to another file to make copy and paste easier


- Maybe also add a replacer that replace add concat to the policies
- Maybe also add a replace that replace all LA parameters to something that use LA_general_info




# TODOs:
- have a check to empty or null values
- need to remove the dependencies in the splited files
- add variabls to AI








