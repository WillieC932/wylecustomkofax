'****************************************************************************
'*	(c) Copyright Kofax, Inc. 2009. All rights reserved.
'*	Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************

Imports Kofax.CapLib4.Interop

''' <summary>
''' Common Utilities
''' </summary>
''' <remarks></remarks>
Module Common

    '*************************************************
    ' UnsupportedPDFDected
    '-------------------------------------------------
    ' Purpose:  Determines if the passed in ImageType corresponds to an unsuported
    '           Adobe Acrobat PDF Format (2.x or 3.x).
    ' Input:    nImageType - the value that will be checked to see if it is one of the
    '                        unsupported values.
    ' Note:     This is used by the runtime and by the setup ui logic.
    '*************************************************
    Public Function UnsupportedPDFDected(ByVal nImageType As Integer) As Boolean
        Dim bUnspportedPDF As Boolean = False
        Select Case nImageType
            Case CAP_IMAGE_FORMAT.CAP_FORMAT_PDF, _
                 CAP_IMAGE_FORMAT.CAP_FORMAT_PDF_JPEG, _
                 CAP_IMAGE_FORMAT.CAP_FORMAT_PDF_MULTI, _
                 CAP_IMAGE_FORMAT.CAP_FORMAT_PDF_PCX, _
                 CAP_IMAGE_FORMAT.CAP_FORMAT_PDF_SINGLE
                bUnspportedPDF = True
            Case Else
                bUnspportedPDF = False
        End Select
        Return bUnspportedPDF
    End Function

End Module
