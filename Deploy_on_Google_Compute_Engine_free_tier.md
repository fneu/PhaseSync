# Hosting on Google Cloud Engine

## Enable APIs

- [Compute Engine](https://cloud.google.com/compute)
- [Artifact Registry](https://cloud.google.com/artifact-registry)

## Create a new project in Google Cloud Console (top left corner dropdown)

- note the *Project ID* below the name field.
- In the GitHub repository settings (secrets and variables),
    add the variable `GC_PROJECT_ID` containing the Project ID.

## Generate service account key

- Install [gcloud cli](https://cloud.google.com/sdk/gcloud)
- run `gcloud auth login` to authenticate gcloud client
- run `gcloud iam service-accounts list --project <Project ID>`
to obtain the email address associated with the service account
- Generate a key that will go into a github secret:
```
gcloud iam service-accounts keys \
    create service_account_secret.json \
    --iam-account <email address from previous step> \
    --project <Project ID>
```
- remove linebreaks from the json file
    [for reasons](https://github.com/google-github-actions/auth#authenticating-via-service-account-key-json)
- in Github, go to settings->secrets of your project and add the
    json content as `SERVICE_ACCOUNT_KEY`

## In Artifact Registry, create a new repository
- name: save this in the github settings in the `GAR_REPOSITORY` variable
- format: docker
- location: save this in the github settings in the `GAR_LOCATION` variable
- the rest can be left as standard

## Push the container

At this point, it makes sense to run the deploy github action.
It will not complete, as there is no virtual machine instance
to run as of yet. However, the docker image should be built and pushed now.

## Create an instance group and instance template

- group:
    - single instance (min=1, max=1)
    - no autoscaling
    - ports: add port 80
- instance template:
    - zone: save this in the `GCE_ZONE` variable
    - create e2-micro instance, which is within the free tier
    - container: 
        - choose the image that was just pushed
        - add some volume. For example, mount `/mnt/` from the host as `/data/` in the
          container with read/write permissions
        - add the `ConfigurationStrings__DefaultConnection1` environment variable and set it to
          something like `DataSource=/data/app.db;Cache=Shared` to make the db persistent
    - boot drive:
        - should be container optimized os
        - do not delete if the instance is deleted
    - reserve static ip address in network settings
    - enable http traffic in firewall settings

- Save the name of the created instance in the `GCE_INSTANCE` variable

## Load Balancing

The load balancer can take care of ssl certificates for us.
Create a hew global https load balancer

- front-end:
    - protocol: https
    - ip: static
    - certificate: new, make sure DNS A entry of the domain points to static ip just chosen.
    - http-to-https: yes
- back-end:
    - protocol: http
    - instance-group: chose previously created instance group and it's named port 80
    - diagnose: chose port 80

34.107.246.112
## Push new containers

At this point, the site should be available via http
at the IP address of the gce instance.
The deploy github action should succeed
and changes should be applied.
