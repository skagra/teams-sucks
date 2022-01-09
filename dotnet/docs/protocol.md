# Protocol

## Packet Structure

```
Byte offset | 0      | 1      | 2 ... n | 
Function    | Length | OpCode | Payload | 
```

### Notes

* Length does not include the length byte itself.

## Payload Types

* `String` - Ascii character codes.  No length or terminator as length is implied by packet length.

## Operations

| OpCode | Name           | Direction | Payload      | Description                                            |
| ------ | -------------- | --------- | ------------ | ------------------------------------------------------ |
| 0x01   | Bring to Front | Out       | -            | Bring the teams meeting window to the front + maximise |
| 0x02   | Toggle Mute    | Out       | -            | Toggle microphone mute                                 |
| 0x03   | Toggle Video   | Out       | -            | Toggle video camera                                    |
| 0xFF   | Debug          | Out       | String       | Send a debug message                                   |
