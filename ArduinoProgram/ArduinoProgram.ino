#include <SPI.h>
#include <Ethernet.h>
#include <EthernetUdp.h>
#include <DHT.h>

byte mac[] = {0xD0, 0xA6, 0xC3, 0xE4, 0xD2, 0x5F};

unsigned int receivePort = 4567;

EthernetUDP Udp;

IPAddress serverIP(192, 168, 188, 43);

const int fanPin = 2;

#define DHTPIN 5
#define DHTTYPE DHT22

DHT dht(DHTPIN, DHTTYPE);

float tempOnThreshold = 24.0;
float tempOffThreshold = 22.0;
bool isFanOn = false;
bool isAuto = false;

void setup() {
  Serial.begin(9600);
  while (!Serial) {
  }
  Serial.println("Initialize Ethernet with DHCP:");
  
  if (Ethernet.begin(mac) == 0) {
    Serial.println("Failed to configure Ethernet using DHCP");
    if (Ethernet.hardwareStatus() == EthernetNoHardware) {
      Serial.println("Ethernet shield was not found. Sorry, can't run without hardware. :(");
    } else if (Ethernet.linkStatus() == LinkOFF) {
      Serial.println("Ethernet cable is not connected.");
    }
    while (true) {
      delay(1);
    }
  }
  Serial.print("My IP address: ");
  Serial.println(Ethernet.localIP());

  Udp.begin(receivePort);

  pinMode(fanPin, OUTPUT);
  digitalWrite(fanPin, LOW);

  dht.begin();
}

void loop() {
  int packetSize = Udp.parsePacket();
  if (packetSize) {
    char packetBuffer[255];
    int len = Udp.read(packetBuffer, 255);
    if (len > 0) {
      packetBuffer[len] = '\0'; // Null-terminate the string
      Serial.print("Received packet: ");
      Serial.println(packetBuffer);

      char* command = strtok(packetBuffer, " ");
      char* valueStr = strtok(NULL, " ");
      
      if (command != NULL) {
        if (strcmp(command, "initialize") == 0) {
          String ip = toString(Ethernet.localIP());

          IPAddress serverIP = Udp.remoteIP();
          unsigned int remotePort = Udp.remotePort();
          Udp.beginPacket(serverIP, remotePort);
          Udp.write(ip.c_str());
          Udp.endPacket();
          Serial.println("Sent test message response.");
        } 
        else if (strcmp(command, "data") == 0) {
          float temperature = dht.readTemperature();
          float humidity = dht.readHumidity();
          if (isnan(temperature) || isnan(humidity)) {
            Serial.println("Failed to read from DHT sensor!");
            return; 
          }

          String temperatureString = String(temperature, 2);
          String humidityString = String(humidity, 2); 

          char response[50];
          snprintf(response, sizeof(response), "%s,%s", temperatureString.c_str(), humidityString.c_str());

          IPAddress serverIP = Udp.remoteIP();
          unsigned int remotePort = Udp.remotePort();
          Udp.beginPacket(serverIP, remotePort);
          Udp.write(response);
          if (Udp.endPacket() == 0) {
            Serial.println("Failed to send packet");
          } else {
            Serial.println("Packet sent successfully");
          }
        } 
        else if (strcmp(command, "on") == 0) {
          digitalWrite(fanPin, LOW);
          isFanOn = true;
          Serial.println("Fan is ON");
        } 
        else if (strcmp(command, "off") == 0) {
          digitalWrite(fanPin, HIGH);
          isFanOn = false;
          Serial.println("Fan is OFF");
        }
        else if (strcmp(command, "autoModeOn") == 0) {
          isAuto = true;
          Serial.println(isAuto);
          Serial.println("auto mode is On");}
        else if (strcmp(command, "autoModeOff") == 0){
            isAuto = false;
            digitalWrite(fanPin, HIGH);
            Serial.println("Auto mode is Off");
        }
        else if (strcmp(command, "setTresholdOn") == 0 && valueStr != NULL) {
          float newThreshold = atof(valueStr);
          if (newThreshold != 0) {
            tempOnThreshold = newThreshold;
            Serial.print("Set new On threshold: ");
            Serial.println(tempOnThreshold);
          } else {
            Serial.println("Invalid On threshold value");
          }
        } 
        else if (strcmp(command, "setTresholdOff") == 0 && valueStr != NULL) {
          float newThreshold = atof(valueStr);
          if (newThreshold != 0) {
            tempOffThreshold = newThreshold;
            Serial.print("Set new Off threshold: ");
            Serial.println(tempOffThreshold);
          } else {
            Serial.println("Invalid Off threshold value");
          }
        }
        else {
          Serial.println("Invalid command");
        }
      }
    }
  }

  // Automatic fan control based on temperature
  float currentTemperature = dht.readTemperature();
  if (isAuto == true){
    if (!isnan(currentTemperature)) {
      if (currentTemperature > tempOnThreshold && !isFanOn) {
        digitalWrite(fanPin, LOW); // Turn the fan on
        isFanOn = true;
        Serial.println("Automatic: Fan is ON");
      } else if (currentTemperature < tempOffThreshold && isFanOn) {
        digitalWrite(fanPin, HIGH); // Turn the fan off
        isFanOn = false;
        Serial.println("Automatic: Fan is OFF");
      }
    } else {
      Serial.println("Failed to read temperature for automatic control");
    }
  }

  delay(1000);
}

String toString(const IPAddress& address){
  return String() + address[0] + "." + address[1] + "." + address[2] + "." + address[3];
}
