# IdentityServicePrototype
## Setup
`identity.api` requires RSA key pairs (private key and public key) to function. The keys should be placed in the `identity.api\Keys` directory before running the application.

    ├── identity.api
        ├── Keys
           ├── PrivateKey.pem
           ├── PublicKey.pem

### Generate RSA key
#### Step 1: Generate `private key`
```
openssl genrsa -out PrivateKey.pem 2048
```
#### Step 2: Generate `public key` from `private key`
```
openssl rsa -in PrivateKey.pem -pubout -out PublicKey.pem
```
