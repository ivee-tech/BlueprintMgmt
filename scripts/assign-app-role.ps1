$roleDef = 'Owner'

$mgmtGroupId = ''
$subscriptionId = ''
$appId = ''
$subScope = '/subscriptions/' + $subscriptionId
$mgmtGroupScope = '/providers/Microsoft.Management/managementGroups/' + $mgmtGroupId

New-AzRoleAssignment -RoleDefinitionName $roleDef -ApplicationId $appId -Scope $subScope
New-AzRoleAssignment -RoleDefinitionName $roleDef -ApplicationId $appId -Scope $mgmtGroupScope

$azureBlueprintAppId = 'f71766dc-90d9-4b7d-bd9d-4499c4331c3f'  #Azure Blueprint application (static Application ID)
New-AzRoleAssignment -RoleDefinitionName $roleDef -ApplicationId $azureBlueprintAppId -Scope $subScope


