import StylizedButton from "./StylizedButton"
import StylizedText from "./StylizedText"

interface ConnectionFailedProps {
    text: string
    customStyles?: string
    functionCallback: () => void
}

export default function ConnectionFailed({ text, customStyles, functionCallback }: ConnectionFailedProps) {
    return (
        <div>
            <StylizedText 
                text={text}
                size="medium"
                fontThickness="font-semibold"
                color="semi-black"
                customStyles={`text-center ${customStyles} mb-5`}
            />
            <StylizedButton 
                isSelected={true}
                functionCallback={functionCallback}
                text="Retry"
                textSelectedColor="text-black-500"
                color="bg-[red]"
                borderSize="medium"
                shadowSize="medium"
            />
        </div>
    )
}