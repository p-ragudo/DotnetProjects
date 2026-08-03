import type { Ticket, WindowWorker } from "../App"

interface WindowProp {
    windowWorker: WindowWorker,
    ticket: Ticket | null,
}

const Window = ({windowWorker, ticket}: WindowProp) => {
  const borderColor = ticket === null ? 'border-green-500' : 'border-red-500'
  const textColor = ticket === null ? 'text-green-500' : 'text-red-500'

  return (
    <div className={`border-solid border-2 ${borderColor} rounded-sm w-100 h-30 flex flex-col text-center`}>
      <p className={`w-full mt-1 border-b-1 border-solid pb-1 ${textColor}`}>Window {windowWorker.windowNumber}</p>
      <div className={`h-full content-center ${textColor}`}>
        {ticket ? (
          <div>
              <p>Ticket: {ticket.id}</p>
              <p>{ticket.status}</p>
          </div>
        ) : (
          <p>Free</p>
        )}
      </div>
    </div>
  )
}

export default Window
