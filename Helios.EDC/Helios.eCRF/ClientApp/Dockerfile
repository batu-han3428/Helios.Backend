FROM node:18.18 as builder

RUN mkdir /usr/src/app
WORKDIR /usr/src/app

ENV PATH /usr/src/app/node_modules/.bin:$PATH

COPY package.json /usr/src/app/package.json

RUN npm install -g npm@10.2.4
RUN npm install --legacy-peer-deps

COPY ./public /usr/src/app/public
COPY ./src /usr/src/app/src

RUN npm run build

# production environment
FROM nginx:1.15.10-alpine
COPY --from=builder /usr/src/app/build /var/www
#COPY ./nginx.conf /etc/nginx/nginx.conf
CMD ["nginx", "-g", "daemon off;"]