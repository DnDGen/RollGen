 
echo "Deploying DnDGen.RollGen to NuGet"

ApiKey=$1
Source=$2

echo "Nuget Source is $Source"
echo "Nuget API Key is $ApiKey (should be secure)"

echo "Pushing DnDGen.RollGen"
dotnet nuget push ./DnDGen.RollGen/bin/Release/DnDGen.RollGen.*.nupkg --api-key $ApiKey --source $Source --skip-duplicate