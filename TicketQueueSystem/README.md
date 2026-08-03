# 🎟️ Queue System

A local real-time queue management webapp with live ticket processing, multi-window worker services, and instant cross-device sync.

**Stack:** React + TypeScript · .NET (ASP.NET Core) · SignalR · Tailwind CSS

---

## How It Works

```
Users → Endpoint/Router → Queue Service → Ticket Handoff Engine → Window Workers (1–3)
Frontend Client (live UI) using SignalR websockets
```

1. Users request tickets through a single API endpoint (`Program.cs`)
2. The **Queue Service** creates and enqueues tickets
3. The **Ticket Handoff Engine** listens to the queue and worker availability continuously
4. **Ticket Handoff Engine** automatically assigns tickets to available window workers — no polling, no manual pull
5. Workers process tickets and update status in real time
6. **SignalR** pushes ticket data and window states to all connected clients instantly

---

## Features

- **Real-time sync** — all connected devices see live updates via WebSocket
- **Auto ticket handoff** — tickets are pushed to workers automatically when a window becomes free
- **3 window workers** out of the box, easily extendable
- **Live dashboard** showing:
  - Tickets currently in queue
  - Total waiting count
  - Completed tickets list
  - Total done count
- **Single-file backend entrypoint** — all services registered in `Program.cs`

---

## Getting Started

### Backend

```bash
cd backend
dotnet run
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

or use:
```bash
npm run dev:localnetwork
```
for exposing app within the same netowrk

SignalR hub connects automatically on frontend load.

---

## Architecture Notes

- The backend uses a **push-based handoff**, not a pull-from-queue pattern. Workers do not poll; the engine dispatches to them.
- Window state (`isAvailable`, `currentTicket`) and ticket queue updates are broadcast over the same SignalR connection.
- All service registration and the single HTTP endpoint live in `Program.cs` for simplicity.