# CryptoPad

A simple, encrypted text editor

## Features

- Insustry standard AES encryption
- Multiple different encryption modes
- Backdoor free
- Code that is easy to verify and audit
- Simple text edit controls comparable to other simple plain text editors.
- Printing
- Portable

## Usage

The editor is mostly used like a regular text editor.
Where possible, it will automatically encrypt and decrypt your files.

## Security

Your text is encrypted using highest grade AES 256 bit encryption.
There are no custom encryption methods or obfuscation techniques in use.

- The mode in use is CBC with PKCS7 padding.
- The encrypted data is secured against tampering by using a SHA256 HMAC.
- Random data is produced using a cryptographically safe number generator.

## How it works

This chapter explains how the encryption, decryption and verification works.

### Encrypting the text

It encrypts your text using randomly generated values (aes key, iv, hmac key) with AES.
The encrypted content is then fed through a HMAC function to detect tampering when decrypting the file.
The AES and HMAC keys are then encrypted using the selected modes (see below).

### Encrypting the keys

The keys are concatenated together into the string `base64(aesKey):base64(hmacKey)`.
This string is then encrypted using all the selected modes.
Encrypting using the local account or machine account use a proprietary windows mechanism.
The keyfile and password methods use AES to encrypt the keys.
The AES key and HMAC key for the key encryption are created from a password derivation function.

This individual mode encryption is the reason why you can decrypt and re-encrypt the file with only one method present.

### Serializing

The encrypted text data and all methods are serialized into an XML file without the plain AES and HMAC keys.

### Decrypting

When the document is first deserialized from the XML,
the methods are checked for tampering and then processed.
Each method is attempted until one succeeds or all failed.
If one method succeeds in decrypting the AES and HMAC keys,
the keys are stored in memory for the duration the file is open.

Next, the encrypted text is validated against the HMAC key and if not tampered with, decrypted into the edit control.

Because the keys are kept in memory, the user can quickly save changes by simply encrypting the new text with the existing key.
This means that no password prompt is necessary.

### Mode preservation

All existing modes are preserved when working with a file,
including those that either didn't work or the editor doesn't knows how to use.

The way the encryption is set up allows tools to perform various actions on the modes:

- Adding new modes at any time
- Changing existing modes at any time
- Removing modes at any time.

### Changing the AES key

Only when the user wants to change the AES key must all methods be re-applied to the new key.
This means discarding unknown methods as they will stop working.

## Encryption modes

This application by default supports multiple encryption modes.
You can enable or disable them individually for each file.
As long as you can use one method to decrypt the file,
you can edit and re-encrypt it without requiring access to the other modes.

This means that the file format allows multiple people to have their own password each for example.

Below is the list of all supported modes.

- *Security* indicates how safe this method is in terms of protecting the keys, provided you use it correctly.
- *Stability* indicates how volatile this method is.
- *Automatic E/D* indicates whether this mode can encrypt and/or decrypt without user interaction.

At least one high stability method should be chosen to increse chances of recovery because of data loss.

### Mode: Local User Account

- Security: High
- Stability: Low
- Automatic E/D: Yes/Yes

This uses the `CryptProtectData` function of Windows.
This allows fully automatic encryption and decryption of the file.
The downside is that it only works on the exact account in the exact state it was created on.
Roaming profiles in an Active Directory domain will work properly.

**CAUTION! This method is rather volatile and could stop working unexpectedly!**

#### Usage:

This method is suitable for quick access to encrypted files.
This means you can use it to avoid typing the password or selecting the key file each time you open the file.
It also works well in combination with the RSA key method if you plan to send a file to somebody.

#### Recommendation:

Use as a convenience only but not as the sole method of encrypting files,
unless the file is useless anyways without access to that specific account.

#### Possible problems:

- If the user name is changed or the password is reset, then this method likely fails to work
- If the user doesn't has a roaming profile, this method is bound to the computer it was used on.
- If the user has a roaming profile and multiple simultaneous sessions, keys can get overwritten on logout.

### Mode: Local Machine Account

- Security: High
- Stability: Medium
- Automatic E/D: Yes/Yes

This is very similar to the local user account encryption,
but uses keys bound to the Windows installation of the computer and not to the user.

**CAUTION! Every user on the computer can decrypt and edit the file**.

#### Usage:

This method is suitable for quick access to encrypted files.
This means you can use it to avoid typing the password or selecting the key file each time you open the file.

#### Recommendation:

Use as a convenience only but not as the sole method of encrypting files,
unless the file is useless anyways without access to that specific computer.
(For example if it contains hardware bound information)

#### Possible problems:

- The key can get lost if your system becomes corrupted.
- Somebody who is able to log into an account or create accounts can decrypt the file.

### Mode: Keyfile

- Security: High
- Stability: Medium
- Automatic E/D: No/No

Uses a key file to encrypt and decrypt the text.
Any file of your liking can be chosen,
this includes files like music, video or even applications.

#### Usage:

This method is suitable as an alternative to password based methods.
In fact it's merely a wrapper for the password function that supplies the entire file content as a password.

#### Recommendation:

Use if you don't want to type a password.
Use a unique file that is extremely unlikely to be modified or has been specifically generated to be used as a key file.

#### Possible problems:

The encrypted XML file **does not** contain hints on what file you used.
It's your responsibility to remember what file you used as a key file.

If you use a file from the internet, other people can possibly decrypt your file too.
The keyfile might also disappear, so don't depend on a service to be available.

To decrypt the text, the file used to encrypt must still exist unmodified.
Some applications like to modify files.
For example office documents might get changed when you open them and accidentally save,
even if you did not change anything.
Modern media players might add metadata they discover over the internet.

The safety depends on the chosen file.
You should pick a file that is unique and contains more data than you would type as a password.
Metadata like the file name or modification time **are not** used.
This means for example that if you select a Windows internal executable as a key file,
anybody with a copy of the same Windows version can potentially decrypt the file.

If you lose the file, you lose access to the encrypted text.
You should back up the file to a secure place.

Storing the file on the same computer as the encrypted text is not a good idea.
Store it on removable media and keep that in a safe location.

### Mode: Password

- Security: Medium
- Stability: High
- Automatic E/D: No/No

Regular password based encryption as is very common.
The security depends on the chosen password.

The paramters for processing the password have been chosen much higher than usual.
You might notice this as a slight delay when you save/load a file for the first time.
This is not an excuse to choose a weak password but merely making this future proof.

#### Usage:

Use as a backup method to gain access to an encrypted text file.
This method is extremely unlikely to ever fail.
Only way this can fail is if you forget the password or the encrypted file itself becomes corrupt.

#### Recommendation:

Chose a password that ideally is at least 10 characters long and is not a well known password.

#### Possible problems:

Short passwords, or passwords previously discovered in data breaches significantly weaken the encryption.
Forgotten passwords can't be restored.

### Mode: RSA

- Security: High
- Stability: High
- Automatic E/D: No/Yes

This is similar to the "Keyfile" method but uses RSA encryption.
You can only use RSA keys and not arbitrarily chosen files.
A key generator is provided whenever you save a file.
Decryption of an RSA protected file is automatically attempted with all keys in the key storage.

#### Usage:

This mode is recommended if you want to send an encrypted file to someone else.
The recipient can generate an RSA key and give you the public key part of it.
You can use this key to encrypt the file. Only the recipient can decrypt it now.

#### Recommendation:

Use to securely send a text file to somebody,
or use for yourself as an alternative to the keyfile method.

#### Possible problems:

An RSA key has a "private" and a "public" part.
The public part is used to encrypt, the private part to decrypt.
This means if you possess a key that only has the public part,
you can encrypt files that you can't decrypt again.

The application will warn you if you try to pick such a key
but will allow you to use it this way (see "Usage" above).

## Key recovery

There are no backdoors and/or master keys hardcoded into the application **and there never will be any**.
If every single configured mode of a file is unable to decrypt it, you can consider the content lost.
We plan on providing many modes of various design to increase your chance in key recovery.

Please do not contact me about files you're not able to decrypt.

**I AM NOT ABLE TO BRING BACK ENCRYPTED CONTENT YOU LOST THE KEYS FOR!**

This application will eventually get a mode/configuration that allows some form of recovery,
mainly to make it suitable for a corporate environment.

If you use the keyfile or RSA method, back up the key files to a safe location.
If you use the password method, ensure there's a way to get your password back if you forget it.

## TODO

These items are not yet implemented, but not confirmed to be implemented ever either.
Those that have been implemented will be moved to the bottom.

- [ ] Help
- [ ] User defined encrypted and unencrypted metadata
- [ ] Compression
- [ ] Password prompt confirmation
- [ ] Custom modes via plugin system
- [ ] Certificate based encryption (essentially RSA via the Windows certificate store)
- [ ] Duplicate modes (for example multiple passwords)
- [ ] Text formatting (probably RTF)
- [X] RSA encryption
