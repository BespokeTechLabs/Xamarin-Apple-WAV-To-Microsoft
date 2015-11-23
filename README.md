# Xamarin-Apple-WAV-To-Microsoft
Convert a WAVs header file generated in Apple's AVAudioRecorder to Headers supported by Microsoft.

## The purpose of this repo.
WAV files produced through Apple's AVAudioRecorder have longer headers than some standard codecs support.

Microsoft generally use headers in which are 44 bytes long.
Headers produced by Apple's framework are 4096 bytes long and padded out with 0x00's

The module provided in this repository passes in a file extension such as:
recording.wav
and creates a duplicate file with Apples header converted to a 44 byte one:
recording.wav2
