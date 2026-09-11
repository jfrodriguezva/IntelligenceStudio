const concepts = [
  { title: "Portero", focus: "Inicio de juego, altura de intervención y protección del área.", question: "¿Cuándo atrae la presión y cuándo juega directo?" },
  { title: "Central", focus: "Salida, vigilancia de espalda y defensa de área.", question: "¿Quién fija al delantero y quién salta a la presión?" },
  { title: "Lateral", focus: "Amplitud, apoyo interior y retorno defensivo.", question: "¿Qué espacio deja cuando se incorpora?" },
  { title: "Mediocentro", focus: "Orientación, coberturas y ritmo de circulación.", question: "¿Puede recibir de cara bajo presión?" },
  { title: "Interior", focus: "Llegada, ocupación de intervalos y presión tras pérdida.", question: "¿Qué línea rompe con o sin balón?" },
  { title: "Extremo", focus: "Uno contra uno, amplitud y ataque del segundo palo.", question: "¿Fija al lateral o ataca el intervalo?" },
  { title: "Delantero", focus: "Fijación, apoyos y ataque de profundidad.", question: "¿Cómo crea espacio para los llegadores?" }
];

export default function TacticalLibraryPage() {
  return <main><p className="eyebrow">Biblioteca táctica</p><h1>Funciones y movimientos</h1><p>Guía de observación para transformar lo que ves en notas, evidencia y análisis.</p><ul>{concepts.map((concept) => <li key={concept.title}><h2>{concept.title}</h2><p>{concept.focus}</p><strong>Pregunta de análisis:</strong> {concept.question}</li>)}</ul></main>;
}
