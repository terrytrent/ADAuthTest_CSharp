# AD Authentication Test using C*#*

##Console application testing AD Authentication.

* Pulls DNS servers from first Ethernet interface
* Uses the DnDNS (https://dndns.codeplex.com/) Library to pull the local (on network) LDAP server from DNS, looking up SRV records
* When typing password, random number (from 1 to 3) of Asterisks (*) are generated for each keystroke to obfiscate the number of characters in your password
* Confirms user attempting login is authorized, using List<T> for simulated authorization
* If there is an error returned from LDAP it is parsed and the error message returned to the user
