[string]$projectName = Read-host "Project Name";

if ($projectName -eq '') {
    echo "You have to input your project name!";
} elseif ((Get-ChildItem .\ -recurse | Where-Object {$_.PSIsContainer -eq $true -and $_.Name -eq $projectName}).count -ne '0') {
    echo "'$projectName' Project has already exists!";
} else {
    echo "'$projectName' Project Creating...";

    dotnet new console --name $projectName 

    mv $projectName/Program.cs $projectName/$projectName.cs

    cd $projectName

    dotnet add package RabbitMQ.Client

    dotnet add package CommandLineParser

    cd ..

    echo "'$projectName' Project is Created!";
}