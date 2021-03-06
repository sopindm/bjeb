if [ "$1" = "clean" ]; then
   echo "Cleaning..."

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
   exit 0
fi

echo "Building library"
 
xbuild bjebLib.csproj /p:Configuration=Release /p:BaseIntermediateOutputPath=bin/obj/lib/ /verbosity:quiet /nologo

if [ $? != 0 ]; then
 echo "Library build failed"
 exit 1
fi

echo "Building server" 

xbuild bjebServer.csproj /p:Configuration=Release /p:BaseIntermediateOutputPath=bin/obj/server/ /verbosity:quiet /nologo

if [ $? != 0 ]; then
  echo "Server build failed"
exit 1
fi

echo "Building test client" 

xbuild bjebTest.csproj /p:Configuration=Release /p:BaseIntermediateOutputPath=bin/obj/client/ /verbosity:quiet /nologo

if [ $? != 0 ]; then
 echo "Test client build failed"
 exit 1
fi
