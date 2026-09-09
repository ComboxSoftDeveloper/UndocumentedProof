@echo off
chcp 866 >nul
setlocal enabledelayedexpansion

rem Весь прогон одной командой. Складывает всё в Results\<имя машины>.

set NAME=%COMPUTERNAME%
set OUT=Results\%NAME%
if not exist "Results" mkdir "Results"
if not exist "%OUT%" mkdir "%OUT%"
if not exist "%OUT%" (
    echo Не удалось создать папку %OUT%
    goto fail
)

echo Всё ляжет в %OUT%
echo.

echo === Сборка ===
dotnet build -c Release -warnaserror
if errorlevel 1 goto fail

echo.
echo === Сверка на трёх рантаймах ===
for %%T in (net8.0 net9.0 net10.0) do (
    echo   %%T
    dotnet run -c Release -f %%T --no-build -- checks > "%OUT%\checks_%%T.txt"
    if errorlevel 1 (
        echo   сверка не прошла, смотри "%OUT%\checks_%%T.txt"
        set FAILED=1
    )
)

echo.
echo === Отчёты ===
for %%T in (net8.0 net9.0 net10.0) do (
    for %%R in (keywords arglist swap boolops nullcall) do (
        echo   %%T %%R
        dotnet run -c Release -f %%T --no-build -- %%R > "%OUT%\%%R_%%T.txt"
    )
)

rem Подмена таблицы методов трогает кучу, поэтому она проверяется ещё и на
rem серверном сборщике: у него другая раскладка сегментов.
echo.
echo === На серверном сборщике ===
set DOTNET_gcServer=1
for %%T in (net8.0 net9.0 net10.0) do (
    echo   %%T
    dotnet run -c Release -f %%T --no-build -- swap > "%OUT%\swap_%%T_servergc.txt"
)
set DOTNET_gcServer=

rem Без многоуровневой компиляции методы сразу собираются полностью.
rem Проверка соглашения вызова при этом происходит раньше, и стоит убедиться,
rem что отчёт про переменное число аргументов ведёт себя так же.
echo.
echo === Без многоуровневой компиляции ===
set DOTNET_TieredCompilation=0
for %%T in (net8.0 net9.0 net10.0) do (
    echo   %%T
    dotnet run -c Release -f %%T --no-build -- arglist > "%OUT%\arglist_%%T_notiered.txt"
)
set DOTNET_TieredCompilation=

echo.
if defined FAILED (
    echo Готово, но сверка не прошла хотя бы на одном рантайме.
    echo Поведение изменилось, смотри checks_*.txt в %OUT%
    echo Отчёты при этом сняты полностью.
) else (
    echo Готово. Всё лежит в %OUT%
)
goto end

:fail
echo.
echo Прогон остановлен.
endlocal
exit /b 1

:end
endlocal
