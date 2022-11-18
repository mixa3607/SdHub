#!/bin/bash

COUNTRY="RU"
STATE_PROVINCE="none"
LOCALITY="none"
ORG_NAME="none"
ORG_UNIT_NAME="none"
COMMON_NAME="none"
EMAIL="none"
if [ "$PASSWORD" == "" ]; then
    echo "USE DEFAULT PASSWD!!!!"
    PASSWORD="DeflPasswd"
fi

function generate () {
    mkdir assets
    mkdir assets/certificates

    for CERT_TARGET in "dev" "production"; do
        openssl genrsa -des3 -passout pass:$PASSWORD -out "assets/${CERT_TARGET}_server.pass.key" 2048
        openssl rsa -passin pass:$PASSWORD -in "assets/${CERT_TARGET}_server.pass.key" -out "assets/${CERT_TARGET}_server.key"
        openssl req -new -key "assets/${CERT_TARGET}_server.key" -out "assets/${CERT_TARGET}_server.csr"  -subj "/C=$COUNTRY/ST=$STATE_PROVINCE/L=$LOCALITY/O=$ORG_NAME/OU=$ORG_UNIT_NAME/CN=$COMMON_NAME/emailAddress=$EMAIL"
        openssl x509 -req -sha256 -days 730 -in "assets/${CERT_TARGET}_server.csr" -signkey "assets/${CERT_TARGET}_server.key" -out "assets/${CERT_TARGET}_server.crt"
        openssl aes-256-cbc -k $PASSWORD -in "assets/${CERT_TARGET}_server.key" -out "assets/certificates/${CERT_TARGET}_server.key.enc" -e -md sha256 -pbkdf2 -iter 100000
        openssl pkcs12 -export -passout pass:$PASSWORD -out "assets/certificates/${CERT_TARGET}_server.pfx" -inkey "assets/${CERT_TARGET}_server.key" -in "assets/${CERT_TARGET}_server.crt"
    done
}

generate
