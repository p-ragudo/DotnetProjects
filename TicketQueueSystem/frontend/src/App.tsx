import { useState, useEffect } from 'react'
import './App.css'
import { useSignalR } from './context/SingalRContext'
import Window from "./components/Window"

export type Ticket = {
  id: string,
  userId: string | null,
  millisToComplete: number,
  windowNumber: number | null,
  status: string
}

export type WindowWorker = {
  windowNumber: number,
  isAvailable: boolean,
  currentTicket: Ticket | null
}

type TransactionStatusDto = {
  ticket: Ticket,
  windowWorker: WindowWorker
}

type QueueInfoDto = {
  queue: Ticket[],
  totalWaiting: number
}

type InitialProgramStatusDto = {
  queue: Ticket[],
  doneTickets: Ticket[],
  windowWorkers: WindowWorker[],
  totalWaiting: number
}

function App() {
  const baseApiUrl = import.meta.env.VITE_API_URL
  const windows = [1, 2, 3]

  const { connection } = useSignalR();

  const [queue, setQueue] = useState<Ticket[]>([]);
  const [totalWaiting, setTotalWaiting] = useState<number>(0)
  const [completedTickets, updateCompletedTickets] = useState<Ticket[]>([])
  const [windowWorkers, setWindowWorkers] = useState<WindowWorker[]>(
    windows.map(num => ({
      windowNumber: num,
      isAvailable: true,
      currentTicket: null
    }))
  )
  
  useEffect(() => {
    if(!connection) return

    connection.on("SendTransactionStatus", (dto: TransactionStatusDto) => {
      const { ticket, windowWorker } = dto

      updateCompletedTickets(prev => {
        if(ticket.status === "Done") {
          return [...prev, ticket]
        } else {
          return prev
        }
      })

      setWindowWorkers(prev => {
        const exists = prev.some(ww => ww.windowNumber === windowWorker.windowNumber)

        if(exists) {
          return prev.map(ww => ww.windowNumber === windowWorker.windowNumber 
            ? { ...windowWorker }
            : ww)
        }

        return [...prev, windowWorker]
      })
    })

    connection.on("SendQueueStatus", (dto: QueueInfoDto) => {
      const { queue, totalWaiting } = dto

      setQueue(queue)
      setTotalWaiting(totalWaiting)
    })

    connection.on("SendInitialProgramStatus", (dto: InitialProgramStatusDto) => {
      const { queue, doneTickets, windowWorkers, totalWaiting } = dto

      setQueue(queue)
      updateCompletedTickets(doneTickets)
      setWindowWorkers(windowWorkers)
      setTotalWaiting(totalWaiting)
    })

    return () => {
      connection.off("SendTransactionStatus")
      connection.off("SendQueueStatus")
      connection.off("SendInitialProgramStatus")
    }
  }, [connection])

  const handleRequestTicket = async () => {
    try {
      await fetch(`${baseApiUrl}/api/tickets`) 
    } catch(error) {
      console.error("Failed to request a ticket:", error)
    }
  }

  return (
    <div className='p-5 flex flex-col gap-10 mx-30'>
      <div className='flex justify-between'>
        {windowWorkers.map(windowWorker => {
          const ticket = windowWorker.currentTicket

          return (
            <div key={windowWorker.windowNumber}>
              <Window 
                windowWorker={windowWorker} 
                ticket={ticket ?? null} 
              />
            </div>
          )
        })}
      </div>

      <div className='w-full text-center'>
        <button 
          onClick={handleRequestTicket}
          className='px-10 py-2 bg-green-700 text-white rounded-md w-full'
        >
          Request Ticket
        </button>
      </div>

      <div className='grid grid-cols-2 w-full justify-start'>
        <div className='mr-2.5'>
          <div className='flex w-full border-solid border-1 justify-around mb-4'>
            <p>Queue</p>
            <p>Total Waiting: {totalWaiting}</p>
          </div>
          {queue.map(ticket => (
            <div className='pl-16' key={ticket.id}>
              <p>{ticket.id}</p>
            </div>
          ))}
        </div>

        <div>
          <div className='flex w-full border-solid border-1 justify-around mb-4'>
            <p>Done</p>
            <p>Total Done: {completedTickets.length}</p>
          </div>
          {completedTickets.map(ticket => (
            <p className='pl-16'>{ticket.id}</p>
          ))}
        </div>
      </div>

    </div>
  )
}

export default App
