xbuild lib/bjebLib.csproj /p:Configuration=Release
xbuild server/bjebServer.csproj /p:Configuration=Release
./server.sh
