import time
import socket
import ctypes
import threading
import ctypes
import ctypes.wintypes
import time
from ctypes import CDLL

dll_path = 'kmclassdll.dll'
driver_path = b'kmclass.sys'

atak_time=0.25
buff_time=360
pet_time=4000
buff_cnt=0
pet_cnt=0




driver=None
def _left_button_down():
    global driver
    driver.MouseLeftButtonDown()

def _left_button_up():
    global driver
    driver.MouseLeftButtonUp()

def _right_button_down():
    global driver
    driver.MouseRightButtonDown()

def _right_button_up():
    global driver
    driver.MouseRightButtonUp()

def _middle_button_down():
    global driver
    driver.MouseMiddleButtonDown()

def _middle_button_up():
    global driver
    driver.MouseMiddleButtonUp()

def _move_rel(x, y):
    global driver
    driver.MouseMoveRELATIVE(x,y)

def _move_to(x, y):
    global driver
    driver.MouseMoveABSOLUTE(x,y)

def load_driver():
    global driver
    driver = CDLL(dll_path)
    driver.LoadNTDriver('kmclass',driver_path)
    driver.SetHandle()

def unload_driver():
    global driver
    driver.UnloadNTDriver('kmclass')

data=0
s=socket.socket()
host=socket.gethostname()
port=12345
s.bind((host,port))

s.listen(1)
conn,adres=s.accept()


def calistir():
    threading.Timer(0.25,calistir).start()
    global data
    global atak_time
    global buff_time
    global pet_time
    global buff_cnt
    global pet_cnt

    if data =="1":
        _move_to(510,258)
        _left_button_down()
        _left_button_up()
        data=3
            
    if data=="2":
        keyPress(0x04)
        time.sleep(atak_time)
        buff_cnt=buff_cnt+1
        pet_cnt=pet_cnt+1

        if buff_cnt==buff_time:
            time.sleep(2)
            keyDown(0x07)
            time.sleep(2)
            keyUp(0x07)

            time.sleep(2)
            keyDown(0x08)
            time.sleep(2)
            keyUp(0x08)
 
            buff_cnt=0

        if pet_cnt==pet_time:
            keyPress(0x0B)
            pet_cnt=0



def wait_for_buffer_empty():
    dwal=0x02
    while dwal & 0x02:
          dwal= ctypes.windll.inpout32.Inp32(0x64)
        

def keyDown(scancode):
    wait_for_buffer_empty()
    ctypes.windll.inpout32.Out32(0x64, 0xD2)
    wait_for_buffer_empty()
    ctypes.windll.inpout32.Out32(0x60, scancode)

def keyUp(scancode):
    wait_for_buffer_empty()
    ctypes.windll.inpout32.Out32(0x64, 0xD2)
    wait_for_buffer_empty()
    ctypes.windll.inpout32.Out32(0x60, scancode | 0x80)	

def keyPress(scancode):
    keyDown(scancode)
    time.sleep(0.1)
    keyUp(scancode)


def main():
    load_driver()
    global data
    calistir()
    while True:
        data=conn.recv(1024).decode()
        print(data)
        
if __name__ == "__main__":
     main()
    
