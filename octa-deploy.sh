publish_path=../octaAuth-publish

echo "\n \e[36mPublishing to\e[0m ..."
echo $publish_path

cd Auth
dotnet publish -o $publish_path
#cp -r report_templates $publish_path

echo "\n\e[36mSuccesfully Published!\e[0m \n"
#########################################
sleep 2

echo "\n\e[36mLogging Into Chabokan...\e[0m \n\n"
chabok login -u $1 -p $2
cd $publish_path
echo "\n\n \e[36mDeploying...\e[0m"
chabok deploy -s octa-auth

echo "\n\n\e[36mDONE!\n\n\e[0m"
exit
