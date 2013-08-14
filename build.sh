if [ "$1" = "clean" ]; then
   rm -r bin/lib/*
   rm -r bin/obj/lib/*
   rm -r bin/server/*
   rm -r bin/obj/server/*
   rm -r bin/client/*
   rm -r bin/obj/client/*
   rm -r bin/unityLib/*
   rm -r bin/obj/unityLib/*
   rm -r bin/plugin/*
   rm -r bin/obj/plugin/*
   exit 1
fi

xbuild bjebLib.csproj /p:Configuration=Release /p:BaseIntermediateOutputPath=bin/obj/lib/ /verbosity:minimal

if [ $? != 0 ]; then
 echo "Library build failed"
 exit 1
fi

xbuild bjebServer.csproj /p:Configuration=Release /p:BaseIntermediateOutputPath=bin/obj/server/ /verbosity:minimal

if [ $? != 0 ]; then
  echo "Server build failed"
exit 1
fi

xbuild bjebTest.csproj /p:Configuration=Release /p:BaseIntermediateOutputPath=bin/obj/client/ /verbosity:minimal

if [ $? != 0 ]; then
 echo "Test client build failed"
 exit 1
fi

xbuild bjebLibUnity.csproj /p:Configuration=Release /p:BaseIntermediateOutputPath=bin/obj/unityLib/ /verbosity:minimal

if [ $? != 0 ]; then
 echo "Unity library build failed"
 exit 1
fi

xbuild bjebPlugin.csproj /p:Configuration=Release /p:BaseIntermediateOutputPath=bin/obj/plugin/ /verbosity:minimal

if [ $? != 0 ]; then
 echo "Plugin build failed"
 exit 1
fi


