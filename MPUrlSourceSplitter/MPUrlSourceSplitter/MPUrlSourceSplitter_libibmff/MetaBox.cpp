/*
    Copyright (C) 2007-2010 Team MediaPortal
    http://www.team-mediaportal.com

    This file is part of MediaPortal 2

    MediaPortal 2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal 2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal 2.  If not, see <http://www.gnu.org/licenses/>.
*/

#include "StdAfx.h"

#include "MetaBox.h"

CMetaBox::CMetaBox(void)
  : CFullBox()
{
  this->type = Duplicate(META_BOX_TYPE);
  this->handlerBox = NULL;
}

CMetaBox::~CMetaBox(void)
{
  FREE_MEM_CLASS(this->handlerBox);
}

/* get methods */

bool CMetaBox::GetBox(uint8_t *buffer, uint32_t length)
{
  bool result = __super::GetBox(buffer, length);

  if (result)
  {
    uint32_t position = this->HasExtendedHeader() ? BOX_HEADER_LENGTH_SIZE64 : BOX_HEADER_LENGTH;
  }

  return result;
}

CHandlerBox *CMetaBox::GetHandlerBox(void)
{
  return this->handlerBox;
}

/* set methods */

/* other methods */

bool CMetaBox::Parse(const uint8_t *buffer, uint32_t length)
{
  return this->ParseInternal(buffer, length, true);
}

wchar_t *CMetaBox::GetParsedHumanReadable(const wchar_t *indent)
{
  wchar_t *result = NULL;
  wchar_t *previousResult = __super::GetParsedHumanReadable(indent);

  if ((previousResult != NULL) && (this->IsParsed()))
  {
    wchar_t *tempIndent = FormatString(L"%s\t", indent);

    // prepare finally human readable representation
    result = FormatString(
      L"%s\n" \
      L"%sHandler box:%s" \
      L"%s"
      ,
      
      previousResult,
      indent, (this->GetHandlerBox() == NULL) ? L"" : L"\n",
      (this->GetHandlerBox() == NULL) ? L"" : this->GetHandlerBox()->GetParsedHumanReadable(tempIndent)
      );

    FREE_MEM(tempIndent);
  }

  FREE_MEM(previousResult);

  return result;
}

uint64_t CMetaBox::GetBoxSize(void)
{
  return __super::GetBoxSize();
}

bool CMetaBox::ParseInternal(const unsigned char *buffer, uint32_t length, bool processAdditionalBoxes)
{
  FREE_MEM_CLASS(this->handlerBox);
  bool result = __super::ParseInternal(buffer, length, false);

  if (result)
  {
    if (wcscmp(this->type, META_BOX_TYPE) != 0)
    {
      // incorect box type
      this->parsed = false;
    }
    else
    {
      // box is file type box, parse all values
      uint32_t position = this->HasExtendedHeader() ? FULL_BOX_HEADER_LENGTH_SIZE64 : FULL_BOX_HEADER_LENGTH;
      bool continueParsing = (this->GetSize() <= (uint64_t)length);

      if (continueParsing)
      {
        this->handlerBox = new CHandlerBox();
        continueParsing &= (this->handlerBox != NULL);

        if (continueParsing)
        {
          continueParsing &= this->handlerBox->Parse(buffer + position, (uint32_t)this->GetSize() - position);
          if (continueParsing)
          {
            position += (uint32_t)this->handlerBox->GetSize();
          }
        }

        if (!continueParsing)
        {
          FREE_MEM_CLASS(this->handlerBox);
        }
      }

      if (continueParsing && processAdditionalBoxes)
      {
        this->ProcessAdditionalBoxes(buffer, length, position);
      }

      this->parsed = continueParsing;
    }
  }

  result = this->parsed;

  return result;
}