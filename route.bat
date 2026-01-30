@echo off
echo ===== MAESTIA MODE ON =====

echo [1] DHCP sichern...
netsh interface ipv4 set address name="Ethernet 2" source=dhcp
netsh interface ipv4 set dns name="Ethernet 2" source=dhcp

echo [2] Maestia-IP hinzufuegen...
netsh interface ipv4 add address "Ethernet 2" 83.220.134.118 255.255.255.255

echo [3] Client starten...
pause
echo [4] Cleanup: IP entfernen...
netsh interface ipv4 delete address "Ethernet 2" 83.220.134.118

echo [5] DHCP wiederherstellen...
netsh interface ipv4 set address name="Ethernet 2" source=dhcp
netsh interface ipv4 set dns name="Ethernet 2" source=dhcp

echo ===== MAESTIA MODE OFF =====
pause