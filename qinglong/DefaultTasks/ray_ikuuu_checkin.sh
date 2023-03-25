#!/usr/bin/env bash
# new Env("ikuuu每日签到")
# cron 30 9 * * * ray_ikuuu_checkin.sh

. ray_ikuuu_base.sh

cd ./src/Ray.Quartz.Ikuuu
export DOTNET_ENVIRONMENT=Production && \
export Ray_Ikuuu_Run=checkin && \
dotnet run --ENVIRONMENT=Production