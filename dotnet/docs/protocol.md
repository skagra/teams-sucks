# Protocol

## Packet Structure

```
Byte offset | 0      | 1      | 2 ... n | n+1      |
Function    | Length | OpCode | Payload | Checksum |
```

### Notes

* Checksum is `XOR` of the rest of the packet.

## Payload Types

* `String` - Ascii characters.  No length or terminator as lenght is implied by packet length
* `Time` - Seconds since the Unix epoch (1st Jan 1970)
* `Byte` 

## Operations

| OpCode | Name           | Direction | Payload      | Description                                            |
| ------ | -------------- | --------- | ------------ | ------------------------------------------------------ |
| 0x00   | Hello          | Out       | -            | Exchanged as handshake before any other communication  |
| 0x01   | Hello          | In        | Time         | Exchanged as handshake before any other communication  |
| 0x02   | Call started   | In        | -            | Call started                                           |
| 0x03   | Call ended     | In        | -            | Call ended                                             |
| 0x04   | Bring to Front | Out       | -            | Bring the teams meeting window to the front + maximise |
| 0x05   | Toggle Mute    | Out       | -            | Toggle microphone mute                                 |
| 0x06   | Toggle Video   | Out       | -            | Toggle video camera                                    |
| 0xFF   | Debug          | Out       | String       | Send a debug message                                   |


void setup() {
   time_t rawtime = 1262304000;
    struct tm  ts;
    char       buf[80];

    // Format time, "ddd yyyy-mm-dd hh:mm:ss zzz"
    ts = *localtime(&rawtime);
    strftime(buf, sizeof(buf), "%a %Y-%m-%d %H:%M:%S %Z", &ts);
    printf("%s\n", buf);
}

void loop() {
  // put your main code here, to run repeatedly:

}