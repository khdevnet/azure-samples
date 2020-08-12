## Run function v1 in Docker windows container
* Build solution
* Build docker image
```
docker build -t=app/azure:latest .
```
* Run docker container
```
docker run -p 7071:80 -d app/azure:latest
```

### Problems
* Azure function doesn't reachable outside container
