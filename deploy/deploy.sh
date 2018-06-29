 
echo "Deploying RollGen to NuGet"

ApiKey=$1
Source=$2

echo "Nuget Source is $Source"
echo "Nuget API Key is $ApiKey (should be secure)"

echo "Packing RollGen"
nuget pack ./RollGen/RollGen.nuspec -Verbosity detailed

echo "Pushing RollGen"
nuget push ./DnDGen.RollGen.*.nupkg -Verbosity detailed -ApiKey $ApiKey -Source $Source