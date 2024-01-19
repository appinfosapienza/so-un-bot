git pull origin main
docker stop SOBOT
docker rm SOBOT
docker build -t sobot3 .
docker run -d --name='SOBOT' --net='bridge' -e TZ="Europe/Berlin" -e HOST_OS="Unraid" -e HOST_HOSTNAME="Tower" -e HOST_CONTAINERNAME="SOBOT" -l net.unraid.docker.managed=dockerman -p 8001:8001 -v '/mnt/user/SSD/sobot_data':'/App/ACL':'rw' 'sobot3:latest'
