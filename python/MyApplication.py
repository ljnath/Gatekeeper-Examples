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
        mac_address = uuid.UUID(int=uuid.getnode()).hex[-12:]
        key = sha256(mac_address.encode('utf-8')).hexdigest()
        return key.upper()


class MyApplication:
    """
    Class to generate prime numbers
    """

    def __init__(self, app_token) -> None:
        self.__app_token = app_token

        hardware = Hardware()
        self.__node_id = hardware.get_unique_id()
        self.__application_id = "PY_1d43e73354ba46a6813e52ab47d968f4"
        self.__application_name = "PythonTestApplication"
        self.__application_version = "1.0.0"

    def check_license(self):
        """
        Method to communicate with Gatekeeper server for licensing
        """
        while True:
            try:

                print('\nWaiting for 3 seconds before checking for license...')
                sleep(3)

                url = 'https://app.ljnath.com/gatekeeper/v3/licenses/license'
                data = {
                    "application_id": self.__application_id,
                    "application_name": self.__application_name,
                    "application_description": "This is a gatekeeper example application written in python",
                    "application_version": self.__application_version,
                    "node_id": self.__node_id,
                    "node_description": f"Node with ID {self.__node_id}"
                }

                headers = {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'x-app-token': self.__app_token
                }

                response = requests.post(url, json=data, headers=headers)
                json_response = json.loads(response.text)
                if response.status_code == 200:
                    state = json_response['state']
                    if state == 'ACTIVE':
                        print("License state is active; all good !")
                    elif state == 'EXPIRED':
                        print(f"License has expired. Expiry mesage: {json_response['expiry_message']}")
                    elif state == 'BLOCKED':
                        print("License is blocked")
                    elif state == 'IGNORED':
                        print("License state is ignored")
                else:
                    print(f'Unexpected response code: {response.status_code}. Message: {json_response["message"]}')

            except HTTPError:
                print('Failed to check license status')


if __name__ == '__main__':
    my_application = MyApplication('app-token-created-for-this-application')
    my_application.check_license()
