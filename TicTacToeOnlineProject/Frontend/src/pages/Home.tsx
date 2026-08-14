import { useEffect, useState } from "react";
import { useSignalR } from "../context/SignalRContext";
import StylizedButton from "../components/StylizedButton";

export type PlayerMark = 'X' | 'O' | '';

export interface BoardDto {
  boardId: string
  grid: string[]
  currentTurn: PlayerMark
}

export interface CreateGameResponse {
  success: boolean
  status: string
  boardDto: BoardDto | null
}

export default function Home() {
  const { connection, isConnected } = useSignalR();
  const [tab, setTab] = useState<"create" | "join">("create");
  const [roomCode, setRoomCode] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (!connection || !isConnected) return

  }, [connection, isConnected])

  const handleCreateRoom = async () => {
    try {
      setIsLoading(true)

      const response = await connection?.invoke<CreateGameResponse>("CreateGame")

      if (response?.success) {
        console.log("Game created with ID: ", response.boardDto?.boardId)
      } else {
        console.error("Failed to create game: ", response?.status)
      }
    } catch (err) {
      console.error('Error invoking CreateGameAsync at Home.tsx:', err);
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="flex flex-col items-center gap-4">
      {/* Main Card Container */}
      <div className="w-full rounded-4xl border-[3px] border-[#1E293B] bg-white p-8 shadow-[8px_8px_0px_0px_#1E293B]">
        {/* Header Section */}
        <div className="mb-6 flex items-center gap-4">
          <div className="flex h-14 w-14 items-center justify-center rounded-2xl border-[2.5px] border-[#1E293B] bg-[#FACC15] shadow-[3px_3px_0px_0px_#1E293B]">
            <svg
              className="h-8 w-8 text-[#1E293B]"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2.5"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              <line x1="8" y1="3" x2="8" y2="21" />
              <line x1="16" y1="3" x2="16" y2="21" />
              <line x1="3" y1="8" x2="21" y2="8" />
              <line x1="3" y1="16" x2="21" y2="16" />
            </svg>
          </div>
          <div>
            <h1 className="text-2xl font-black tracking-tight text-[#1E293B]">
              Tic Tac Toe!
            </h1>
            <p className="text-sm font-semibold text-slate-500">
              play a friend, anywhere
            </p>
          </div>
        </div>

        {/* Tab Switcher */}
        <div className="mb-6 flex rounded-xl border-[2.5px] border-[#1E293B] bg-[#FFFBEA] p-1 shadow-[2px_2px_0px_0px_#1E293B]">
          <StylizedButton
            functionCallback={() => setTab("create")} 
            isVisible={tab === "create"}
            text="Create room"
            color="bg-[#FACC15]"
            largeBorder={false}
            hasHover={false}
          />
          <StylizedButton
            functionCallback={() => setTab("join")} 
            isVisible={tab === "join"}
            text="Join room"
            color="bg-[#FACC15]"
            largeBorder={false}
            hasHover={false}
          />
        </div>

        {/* Tab Content */}
        {tab === "create" ? (
          <div>
            <p className="mb-6 text-sm font-semibold leading-relaxed text-slate-700 text-center">
              Get a room code to send your opponent.
            </p>
            <button
              onClick={handleCreateRoom} 
              className="w-full rounded-xl border-[2.5px] border-[#1E293B] bg-[#FACC15] py-3.5 text-base font-bold text-[#1E293B] shadow-[4px_4px_0px_0px_#1E293B] transition-all hover:-translate-y-0.5 active:translate-x-0.5 active:translate-y-0.5 active:shadow-[2px_2px_0px_0px_#1E293B]"
            >
              Create room
            </button>
          </div>
        ) : (
          <div>
            <p className="mb-3 text-sm font-semibold leading-relaxed text-slate-700 text-center">
              Enter the room code shared by your opponent:
            </p>
            <input
              type="text"
              placeholder="e.g. AAY4Z"
              value={roomCode}
              onChange={(e) => setRoomCode(e.target.value.toUpperCase())}
              className="mb-6 w-full rounded-xl border-[2.5px] border-[#1E293B] bg-[#FFFBEA] p-3 text-center text-lg font-black uppercase tracking-widest text-[#1E293B] placeholder-slate-400 outline-none focus:ring-2 focus:ring-[#FACC15]"
            />
            <button className="w-full rounded-xl border-[2.5px] border-[#1E293B] bg-[#FACC15] py-3.5 text-base font-bold text-[#1E293B] shadow-[4px_4px_0px_0px_#1E293B] transition-all hover:-translate-y-0.5 active:translate-x-0.5 active:translate-y-0.5 active:shadow-[2px_2px_0px_0px_#1E293B]">
              Join room
            </button>
          </div>
        )}

        <p className="mb-3 text-sm font-semibold leading-relaxed text-slate-700 text-center">
          {
            tab === "create"
            ? "Get a room code to send your opponent."
            : "Enter the room code shared by your opponent:"
          }
        </p>

        { tab === "join" &&
          <input
            type="text"
            placeholder="e.g. AAY4Z"
            value={roomCode}
            onChange={(e) => setRoomCode(e.target.value.toUpperCase())}
            className="mb-6 w-full rounded-xl border-[2.5px] border-[#1E293B] bg-[#FFFBEA] p-3 text-center text-lg font-black uppercase tracking-widest text-[#1E293B] placeholder-slate-400 outline-none focus:ring-2 focus:ring-[#FACC15]"
          />
        }

        <StylizedButton
          functionCallback={
            tab === "create"
            ? handleCreateRoom
            : () => console.log("joined room")
          } 
          isVisible={true}
          text={
            tab === "create"
            ? "Create room"
            : "Join room"
          }
          color="bg-[#FACC15]"
          largeBorder={true}
          hasHover={true}
        />
      </div>

      {/* Footer Text */}
      <p className="text-xs font-semibold text-slate-400">
        No account needed — the room code is the only key.
      </p>
    </div>
  );
}