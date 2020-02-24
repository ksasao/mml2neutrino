@echo off
setlocal enabledelayedexpansion
cd /d %~dp0

: Project settings
set BASENAME=%~n1
set OUTPUT_PATH=%~p1
set NumThreads=0

: musicXML_to_label
set SUFFIX=musicxml

: NEUTRINO
set ModelDir=KIRITAN

: WORLD
set PitchShift=1.0
set FormantShift=1.0


echo %date% %time% : start MusicXMLtoLabel
bin\musicXMLtoLabel.exe %1 score\label\full\%BASENAME%.lab score\label\mono\%BASENAME%.lab

echo %date% %time% : start NEUTRINO
bin\NEUTRINO.exe score\label\full\%BASENAME%.lab score\label\timing\%BASENAME%.lab output\%BASENAME%.f0 output\%BASENAME%.mgc output\%BASENAME%.bap model\%ModelDir%\ -n %NumThreads% -t

echo %date% %time% : start WORLD
bin\WORLD.exe output\%BASENAME%.f0 output\%BASENAME%.mgc output\%BASENAME%.bap -f %PitchShift% -m %FormantShift% -o %OUTPUT_PATH%\%BASENAME%.wav -n %NumThreads% -t

echo %date% %time% : end
