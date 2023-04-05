from hashlib import sha256
from time import sleep
from urllib.error import HTTPError
import uuid
import json
import requests


class Hardware:
    def get_unique_id(self) -> str:
        """
        Static method to generate unique hardware ID based on hardware MAC addresss
        """
        mac = uuid.UUID(int=uuid.getnode()).hex[-12:]
        mac_address = ":".join([mac[e:e + 2] for e in range(0, 11, 2)])
        key = sha256(mac_address.encode('utf-8')).hexdigest()
        unique_key = '-'.join([key[:8], key[8:16], key[16:24], key[24:]])
        return unique_key
    

class MyApplication:
    """
    Class to generate prime numbers
    """

    def __init__(self, api_key) -> None:
        self.__api_key = api_key
        
        hardware = Hardware()
        self.__client_id = hardware.get_unique_id()
        self.__application_id = "PY_1d43e73354ba46a6813e52ab47d968f4"
        self.__application_name = "PythonTestApplication"
        self.__application_version = "1.0.0"

    def check_license(self):
        """
        Method to communicate with Gatekeeper server for licensing
        """
        while True:
            try:
                url = 'https://app.ljnath.com/gatekeeper/v3/licenses/license'
                data = {
                    "application_id": self.__application_id,
                    "application_name": self.__application_name,
                    "application_description": "This is a gatekeeper example application written in python",
                    "application_version": self.__application_version,
                    "client_id": self.__client_id,
                    "client_description": "client with ID "+self.__client_id
                }

                headers = {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'x-api-key': self.__api_key
                }

                response = requests.post(url, json=data, headers=headers)
                if response.status_code == 200:
                    json_response = json.loads(response.text)
                    state = json_response['state']
                    if state == 'ACTIVE':
                        print("License state is active; all good !")
                    elif state == 'EXPIRED':
                        print("License has expired. Expiry mesage: " + json_response['expiry_message'])
                    elif state == 'BLOCKED':
                        print("License is blocked")
                    elif state == 'IGNORED':
                        print("License state is ignored")
                        
            except HTTPError:
                print('Failed to check license status')

            print('\nRe-Checking license state in 3 seconds ...')
            sleep(3)

            
if __name__ == '__main__':    
    my_application = MyApplication('your-gatekeeper-api-key')
    my_application.check_license()
