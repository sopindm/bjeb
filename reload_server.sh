xbuild bjebLib.csproj /p:Configuration=Release /p:BaseIntermediateOutputPath=bin/obj/lib/ /verbosity:minimal

if [ $? != 0 ]; then
 echo "Library build failed"
 exit $?
fi

xbuild bjebServer.csproj /p:Configuration=Release /p:BaseIntermediateOutputPath=bin/obj/server/ /verbosity:minimal

if [ $? != 0 ]; then
  echo "Server build failed"
exit $?
fi

xbuild bjebTest.csproj /p:Configuration=Release /p:BaseIntermediateOutputPath=bin/obj/client/ /verbosity:minimal

if [ $? != 0 ]; then
 echo "Test client build failed"
 exit $?
fi

./server.sh
