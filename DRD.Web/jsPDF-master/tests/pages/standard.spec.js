
/* global describe, it, jsPDF, comparePdf */
/**
 * Standard spec tests
 *
 * These tests return the datauristring so that reference files can be generated.
 * We compare the exact output.
 */

describe('Paging functions', () => {
  it('should add new page', () => {
    const doc = jsPDF()
    doc.addPage()
    doc.addPage()
    doc.text('Text that will end up on page 3', 20, 20)
    doc.setPage(1)
    doc.text('Text that will end up on page 1', 20, 20)
    doc.setPage(2)
    doc.text('Text that will end up on page 2', 20, 20)

    comparePdf(doc.output(), '3pages.pdf', 'pages')
  })

  // @TODO: Document
  it('should insert new page at the beginning', () => {
    const doc = jsPDF()
    doc.text('Text that will end up on page 2', 20, 20)
    doc.addPage()
    doc.text('Text that will end up on page 3', 20, 20)
    doc.insertPage(1)
    doc.text('Text that will end up on page 1', 20, 20)

    comparePdf(doc.output(), '3pages.pdf', 'pages')
  })

  // @TODO: Document
  it('should insert new page in the middle', () => {
    const doc = jsPDF()
    doc.text('Text that will end up on page 1', 20, 20)
    doc.addPage()
    doc.text('Text that will end up on page 3', 20, 20)
    doc.insertPage(2)
    doc.text('Text that will end up on page 2', 20, 20)

    comparePdf(doc.output(), '3pages.pdf', 'pages')
  })

  // @TODO: Document
  it('should delete a page in the middle', () => {
    const doc = jsPDF()
    doc.text('Text that will end up on page 1', 20, 20)
    doc.addPage()
    doc.text('Text that will end up on page 2', 20, 20)
    doc.addPage()
    doc.text('This page is being deleted', 20, 20)
    doc.addPage()
    doc.text('Text that will end up on page 3', 20, 20)
    doc.deletePage(3)
    comparePdf(doc.output(), '3pages.pdf', 'pages')
  })

  // @TODO: Document
  it('should insert two pages and make them swap places', () => {
    const doc = jsPDF()
    doc.text('Text that will end up on page 2', 20, 20)
    doc.addPage()
    doc.text('Text that will end up on page 1', 20, 20)
    doc.movePage(2, 1)

    comparePdf(doc.output(), '2pages.pdf', 'pages')
  })

  it('should insert two pages and make them swap places', () => {
    const doc = jsPDF()
    doc.text('Text that will end up on page 2', 20, 20)
    doc.addPage()
    doc.text('Text that will end up on page 1', 20, 20)
    doc.movePage(1, 2)

    comparePdf(doc.output(), '2pages.pdf', 'pages')
  })
})
