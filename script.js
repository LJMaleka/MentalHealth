const startButton = document.getElementById('start-btn')
const nextButton = document.getElementById('next-btn')
const questionContainerElement = document.getElementById('question-container')
const questionElement = document.getElementById('question')
const answerButtonsElement = document.getElementById('answer-buttons')

let shuffledQuestions, currentQuestionIndex

startButton.addEventListener('click', startGame)
nextButton.addEventListener('click', () => {
  currentQuestionIndex++
  setNextQuestion()
})

function startGame() {
  startButton.classList.add('hide')
  shuffledQuestions = questions.sort(() => Math.random() - .5)
  currentQuestionIndex = 0
  questionContainerElement.classList.remove('hide')
  setNextQuestion()
}

function setNextQuestion() {
  resetState()
  showQuestion(shuffledQuestions[currentQuestionIndex])
}

function showQuestion(question) {
  questionElement.innerText = question.question
  question.answers.forEach(answer => {
    const button = document.createElement('button')
    button.innerText = answer.text
    button.classList.add('btn')
    if (answer.correct) {
      button.dataset.correct = answer.correct
    }
    button.addEventListener('click', selectAnswer)
    answerButtonsElement.appendChild(button)
  })
}

function resetState() {
  clearStatusClass(document.body)
  nextButton.classList.add('hide')
  while (answerButtonsElement.firstChild) {
    answerButtonsElement.removeChild(answerButtonsElement.firstChild)
  }
}

function selectAnswer(e) {
  const selectedButton = e.target
  const correct = selectedButton.dataset.correct
  setStatusClass(document.body, correct)
  Array.from(answerButtonsElement.children).forEach(button => {
    setStatusClass(button, button.dataset.correct)
  })
  if (shuffledQuestions.length > currentQuestionIndex + 1) {
    nextButton.classList.remove('hide')
  } else {
    startButton.innerText = 'Restart'
    startButton.classList.remove('hide')
  }
}

function setStatusClass(element, correct) {
  clearStatusClass(element)
  if (correct) {
    element.classList.add('correct')
  } else {
    element.classList.add('wrong')
  }
}

function clearStatusClass(element) {
  element.classList.remove('correct')
  element.classList.remove('wrong')
}

const questions = [
  {
    question: 'What is the most effective treatment for mental health disorders?',
    answers: [
      { text: 'A) Medication', correct: true },
      { text: 'B) Exercise and healthy diet', correct: false },
      { text: 'C) Hypnosis', correct: false },
      { text: 'D) Ignoring the problem and hoping it goes away', correct: false }
    ]
  },
  {
    question: 'What is the main cause of mental health issues?',
    answers: [
      { text: 'A) Genetics and family history', correct: true },
      { text: 'B) Childhood trauma', correct: false },
      { text: 'C) Excessive smartphone use', correct: false },
      { text: 'D) Wearing bright colors', correct: false }
    ]
  },
  {
    question: 'Can mental health disorders only affect adults?',
    answers: [
      { text: 'A) Yes, only adults can experience mental health disorders', correct: false },
      { text: 'B) No, mental health disorders can affect people of all ages', correct: true },
      { text: 'C) Only teenagers can experience mental health disorders', correct: false },
      { text: 'D) Mental health disorders are a myth', correct: false }
    ]
  },
  {
    question: 'Is seeking help for mental health issues a sign of weakness?',
    answers: [
      { text: 'A) Yes, seeking help is a sign of weakness', correct: false },
      { text: 'B) No, seeking help is a sign of strength and self-care', correct: true },
    { text: 'C) It depends on the severity of the issue', correct: false },
    { text: 'D) Seeking help is unnecessary, mental health issues can be solved on their own', correct: false }
    ]
  }
]