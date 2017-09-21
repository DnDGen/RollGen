 
echo "Deploying RollGen to NuGet"

ApiKey=$1
Source=$2

echo "Nuget Source is $Source"
echo "Nuget API Key is $ApiKey (should be secure)"

echo "Packing RollGen"
nuget pack ./RollGen/RollGen.nuspec -Verbosity detailed

echo "Packing RollGen.Domain"
nuget pack ./RollGen.Domain/RollGen.Domain.nuspec -Verbosity detailed

echo "Pushing RollGen"
nuget push ./DnDGen.RollGen.*.nupkg -Verbosity detailed -ApiKey $ApiKey -Source $Source

echo "Pushing RollGen.Domain"
nuget push ./DnDGen.RollGen.Domain.*.nupkg -Verbosity detailed -ApiKey $ApiKey -Source $Source