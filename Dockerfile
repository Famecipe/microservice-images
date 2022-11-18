FROM alpine:3.14
ADD "/bin/Release/net6.0/linux-musl-x64/publish/Famecipe.Microservice.Images" "/bin/Famecipe.Microservice.Images"
RUN apk upgrade --no-cache && \
    apk add --no-cache libgcc libstdc++
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
CMD [ "/bin/Famecipe.Microservice.Images" ]