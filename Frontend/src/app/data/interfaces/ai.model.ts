export interface quetionModel{
    question: string;
}

export interface aiAsistantResponce 
{
    id: string;
    question: string;
    answer: string;
    askedAt: Date;
}
export interface aiAsistantConversation
{
    answer: string;
    askedAt: Date;
}