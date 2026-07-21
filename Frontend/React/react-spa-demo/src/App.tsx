import './App.css'
import { CatalogPage } from './components/CatalogPage'
import { NavLink, Route, Routes } from 'react-router-dom'
import { About } from './pages/About'
import { BookDetail } from './pages/BookDetail'

function App() {

  return (
    <div className="app">
      <header className='app-header'>
        <h1>Library</h1>
        <nav className='app-header'>
          <NavLink to="/">Catalog</NavLink>
          <NavLink to="/about">About</NavLink>
        </nav>

      </header>
      <main>
        <Routes>
          <Route path='/' element={<CatalogPage/>} />
          <Route path='/inventory/:sku' element={<BookDetail/>} />
          <Route path='/about' element={<About />} />
          <Route path='*' element={<p>Page not found</p>} />

        </Routes>
      </main>
    </div>
  )
}

export default App
