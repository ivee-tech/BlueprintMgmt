### Azure Blueprints management tool
Helps creating, viewing, publishing, assigning / unassigning, and deleting Azure Blueprints.

Usage:

BlueprintMgmt.exe command [--arg1 value1[, --arg2 value2[...]]]

command:
 - create - creates a blueprint specified by --name argument
 - get - returns the blueprint specified by --name argument
 - get-artifacts - returns the blueprint artifacts specified by --name argument
 - get-artifact - returns the blueprint artifact specified by --name and --artifact-name argument
 - publish - publihes a blueprint specified by --name and --version arguments
 - assign - assigns a blueprint specified by --name argument
 - unassign - unassigns a blueprint specified by --name and --assignment-name arguments
 - delete - deletes a blueprint specified by --name argument
 - help - displays this screen

Examples

 - creates Blueprint blueprint1234

BlueprintMgmt.exe create --name blueprint1234

 - returns Blueprint blueprint1234

BlueprintMgmt.exe get --name blueprint1234

 - returns Artifacts for Blueprint blueprint1234

BlueprintMgmt.exe get-artifacts --name blueprint1234

 - returns Artifact template-storage-account for Blueprint blueprint1234

BlueprintMgmt.exe get-artifact --name blueprint1234 --atifact-name template-storage-account

 - publishes version 1.0 for blueprint1234

BlueprintMgmt.exe publish --name blueprint1234 --version v1.0

 - assigns Blueprint blueprint1234, with assignment name Assignment-blueprint1234

BlueprintMgmt.exe assign --name blueprint1234 --asignment-name Assignment-blueprint1234

 - unassigns Blueprint blueprint1234, based on assignment name

BlueprintMgmt.exe unassign --name blueprint1234 --asignment-name Assignment-blueprint1234

 - deletes Blueprint blueprint1234

BlueprintMgmt.exe delete --name blueprint1234


