# FmsWebApp

# 모니터링 개발용 프로젝트
- typescript 개발은 중지 되었습니다.
- 클라이언트 간 동기화를 위해 렌더링을 위한 데이터를 생성하는 프로그램이 분리되었습니다. (MDC)

# 작업내용
- MDC(Monitoring Data Control) Wrapper 완성(fmswebapp/com.doosan.fms.mdc)
- MDC signalR 클라이언트 완성(fmswebapp/com.doosan.fms.mdc/Tasks/MdcEnginePolling)
- Api Server signalR 서버 완성(fmswebapp/com.doosan.fms.signalRHub/HubServer)
- Api server signalR 브로드캐스팅 모듈 완성(fmswebapp/com.doosan.fms.signalRHub/HubServer & HubClient)
- Javascript signalR Client 완성 (fmswebapp/com.doosan.fms.webapp/Client/wwwroot/fms/worker/fmsMonitoringRenderWorker.js)

# 종속성
- dotnet 5

# 이슈
- FMS 데이터 복잡성 및 개별로직으로 인해 WCS와 분리되었습니다.
- FMS 노드와 버텍스 집합을 그리기 위해 렌더링 툴을 요청받아 해당 프로젝트는 분리되고 이후 통합되어야 합니다.
