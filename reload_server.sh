echo "Building library"

xbuild bjebLib.csproj /p:Configuration=Release /p:BaseIntermediateOutputPath=bin/obj/lib/ /verbosity:minimal /nologo

if [ $? != 0 ]; then
 echo "Library build failed"
 exit $?
fi

echo "Building server"

xbuild bjebServer.csproj /p:Configuration=Release /p:BaseIntermediateOutputPath=bin/obj/server/ /verbosity:minimal /nologo

if [ $? != 0 ]; then
  echo "Server build failed"
exit $?
fi

echo "Building test client"

xbuild bjebTest.csproj /p:Configuration=Release /p:BaseIntermediateOutputPath=bin/obj/client/ /verbosity:minimal /nologo

if [ $? != 0 ]; then
 echo "Test client build failed"
 exit $?
fi

echo "Running server ..."

./server.sh
