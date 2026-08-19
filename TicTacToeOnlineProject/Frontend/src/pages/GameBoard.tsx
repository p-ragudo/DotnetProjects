import { useEffect, useState, useCallback, useRef } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useSignalR } from "../context/SignalRContext";
import StylizedButton from "../components/StylizedButton";
import LoadingContainer from "../components/LoadingContainer";
import type { AssignMarkResponse, BoardDto, JoinGameResponse } from "./Home";
import ConnectionFailed from "../components/ConnectionFailed";

export type MoveReturnStatus = 
  | "MoveSuccess"
  | "BoardNotFound"
  | "BoardNullException"
  | "IndexOutOfRange"
  | "NotCurrentTurn"
  | "CellNotEmpty"
  | "GameFinished"

export interface MoveResponse {
  success: boolean;
  status?: MoveReturnStatus;
  updatedBoardDto?: BoardDto;
  isGameOver: boolean;
  winnerMark: string | null
}


export default function GameBoard() {
   const navigate = useNavigate();
  const { connection, isConnected, isInitialLoading, reconnect } = useSignalR();

  const { roomCode } = useParams<"roomCode">();
  const [boardDto, setBoardDto] = useState<BoardDto | null>(null);
  const [playerMark, setPlayerMark] = useState<string | null>(null);
  const [opponentMark, setOpponentMark] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [currentTurn, setCurrentTurn] = useState<string>("");
  const [winner, setWinner] = useState<string | null>(null);
  const [isGameOver, setIsGameOver] = useState(false);
  const [isWaitingForOpponent, setIsWaitingForOpponent] = useState(true);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isRetrying, setIsRetrying] = useState(false)
  const hasJoinedRef = useRef(false)

  useEffect(() => {
    if (!roomCode) return

    async function initGame() {
      setLoading(true)

      if (!roomCode || !connection || hasJoinedRef.current) return;
      hasJoinedRef.current = true;

      try {
        const joinGameResponse = await connection?.invoke<JoinGameResponse>(
          "JoinGameRoom", roomCode
        )
        if (joinGameResponse == null || !joinGameResponse.success) {
          setErrorMessage("Failed to join game")
          console.error("Failed to join game:", joinGameResponse?.status)
          return
        }

        const getMarkResponse = await connection?.invoke<AssignMarkResponse>(
          "GetMark", roomCode
        )
        if (getMarkResponse == null || !getMarkResponse.success) {
          setErrorMessage("Failed to get assigned mark")
          console.error("Failed to get assigned mark:", getMarkResponse?.status)
          return
        }

        setBoardDto(joinGameResponse.boardDto)
        setCurrentTurn(joinGameResponse.boardDto?.currentTurn!)
        setPlayerMark(getMarkResponse.mark)
        setOpponentMark(playerMark === 'X' ? 'O' : 'X')
      } catch (err) {
        console.error("Error in GameBoard.useEffect.initGame:", err);
      } finally {
        setLoading(false);
      }
    }

    const handleNotifyGroupOnPlayerJoin = () => {
      setIsWaitingForOpponent(false)
    }

    const handleGameUpdated = (boardDto: BoardDto) => {
      setBoardDto(boardDto)
      setCurrentTurn(boardDto.currentTurn)
    }

    const handleGameOver = (boardDto: BoardDto, winnerMark: string) => {
      setBoardDto(boardDto)
      setWinner(winnerMark)
      setIsGameOver(true)
    }

    connection?.on("NotifyGroupOnPlayerJoin", handleNotifyGroupOnPlayerJoin)
    connection?.on("GameUpdated", handleGameUpdated)
    connection?.on("GameOver", handleGameOver)

    initGame()

    return () => {
      connection?.off("NotifyGroupOnPlayerJoin", handleNotifyGroupOnPlayerJoin)
      connection?.off("GameUpdated", handleGameUpdated)
      connection?.off("GameOver", handleGameOver)
    }
  }, [roomCode, connection])

  const isMyTurn = () => currentTurn === playerMark
  const handleLeave = () => navigate("/")

  const handleCellClick = async (index: number) => {
    try {
      const response = await connection?.invoke<MoveResponse>(
        "MakeMove",
        roomCode,
        index,
        playerMark
      )

      if (response == null || !response?.success) {
        setErrorMessage("Failed to execute move. Please try again")
        console.error("Failed to execute move:", response?.status)
        return
      }

      if (response.status === "CellNotEmpty") {
        setErrorMessage("Cell is not empty. Please try again")
        console.error("Invalid player move, cell not empty")
        return
      }

      if (response.status === "NotCurrentTurn") {
        setErrorMessage("Not your turn. Please wait for opponent")
        console.error("Invalid player move, not their current turn")
        return
      }
    } catch (err) {
      console.error("Error in GameBoard.handleCellClick:", err);
    } finally {
      setLoading(false);
    }
  }

  const handleReconnect = async () => {
    setIsRetrying(true);
    
    try {
      await reconnect();
    } catch (err) {
      console.error("Manual reconnect failed:", err);
    } finally {
      setIsRetrying(false);
    }
  };

  if (!isConnected) {
    return (
      <div className="w-full max-w-sm mx-auto flex flex-col items-center">
        <ConnectionFailed text="Failed to connect to the server" functionCallback={handleReconnect} />
      </div>
    );
  }

  if (isInitialLoading || isRetrying) {
    return <LoadingContainer text="Connecting to server" styleSelected="bg-[#FACC15]" />;
  }

  // Only show the loader while the network request is in flight and there's no error
  if (loading && !errorMessage) {
    return <LoadingContainer text="Joining room" styleSelected="bg-[#FACC15]" />;
  }

  return (
    <div className="flex w-full flex-col items-center gap-4">
      {/* Top-Right Leave Button */}
      <div className="fixed top-6 right-6 z-50">
        <StylizedButton
          isSelected={true}
          text="Leave"
          color="bg-[#EF4444]"
          padding="px-4 py-2"
          borderSize="small"
          shadowSize="small"
          customStyles="w-auto"
          functionCallback={handleLeave}
        />
      </div>

      {/* Centered Room Code Header */}
      <div className="flex w-full items-center justify-center px-1">
        <span className="text-md font-bold text-slate-500">
          Room <span className="font-bold text-sm uppercase text-[#1E293B]">{roomCode}</span>
        </span>
      </div>

      {/* Players Header */}
      <div className="grid w-full grid-cols-3 items-center px-2">
        {/* You */}
        <div className="flex flex-col items-start gap-1">
          <div className="flex items-center gap-2">
            <div className="flex h-9 w-9 items-center justify-center rounded-full border-[2.5px] border-[#1E293B] bg-[#3B82F6] font-black text-white shadow-[2px_2px_0px_0px_#1E293B]">
              {playerMark || "?"}
            </div>
            <span className="text-base font-black text-[#1E293B]">You</span>
          </div>
        </div>

        {/* VS */}
        <div className="text-center">
          <span className="text-sm font-extrabold tracking-wider text-slate-400">VS</span>
        </div>

        {/* Opponent */}
        <div className="flex flex-col items-end gap-1">
          <div className="flex items-center gap-2">
            <span className="text-base font-black text-[#1E293B]">Opponent</span>
            <div className="flex h-9 w-9 items-center justify-center rounded-full border-[2.5px] border-[#1E293B] bg-[#EF4444] font-black text-white shadow-[2px_2px_0px_0px_#1E293B]">
              {opponentMark || "?"}
            </div>
          </div>
        </div>
      </div>

      {/* Turn or Game Over Status Pill */}
      <div className="my-1">
        <div
          className={`rounded-2xl border-[2.5px] border-[#1E293B] px-6 py-2 text-sm font-black text-white shadow-[3px_3px_0px_0px_#1E293B] transition-all ${
            isGameOver
              ? "bg-[#10B981]"
              : isMyTurn()
                ? "bg-[#3B82F6]"
                : "bg-slate-400"
          }`}
        >
          {isGameOver 
            ? winner 
            : isMyTurn()
              ? "Your turn" 
              : "Opponent's turn"}
        </div>
      </div>

      {/* 3x3 Grid Container */}
      <div className="aspect-square w-full max-w-95 overflow-hidden rounded-4xl border-[3.5px] border-[#1E293B] bg-white shadow-[8px_8px_0px_0px_#1E293B]">
        <div className="grid h-full w-full grid-cols-3 grid-rows-3">
          {boardDto?.grid.map((cellValue, index) => {
            const isRightBorder = index % 3 !== 2;
            const isBottomBorder = index < 6;

            return (
              <button
                key={index}
                type="button"
                disabled={!isMyTurn || cellValue !== "" || isGameOver}
                onClick={() => handleCellClick(index)}
                className={`flex items-center justify-center transition-colors ${
                  isRightBorder ? "border-r-[3.5px] border-[#1E293B]" : ""
                } ${isBottomBorder ? "border-b-[3.5px] border-[#1E293B]" : ""} ${
                  !cellValue && isMyTurn() && !isGameOver ? "hover:bg-[#FFFBEA]" : ""
                }`}
              >
                {cellValue === "X" && (
                  <span className="text-4xl font-black text-[#3B82F6] sm:text-5xl">
                    X
                  </span>
                )}
                {cellValue === "O" && (
                  <span className="text-4xl font-black text-[#EF4444] sm:text-5xl">
                    O
                  </span>
                )}
              </button>
            );
          })}
        </div>
      </div>

      {/* Dynamic Subtext / Error Display */}
      <p className="mt-1 text-xs font-semibold text-slate-400">
        {errorMessage ? (
          <span className="font-bold text-red-500">{errorMessage}</span>
        ) : isGameOver ? (
          "Game over"
        ) : isWaitingForOpponent ? (
          "Waiting for opponent to join..."
        ) : isMyTurn() ? (
          "Tap a square to play"
        ) : (
          "Waiting for opponent move..."
        )}
      </p>
    </div>
  );
}