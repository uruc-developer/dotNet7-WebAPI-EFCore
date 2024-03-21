global using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        //private static List<Character> characters = new List<Character>{
        //    new Character(),
        //    new Character {Id=1,Name= "Sam"}
        //};
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public CharacterService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = _mapper.Map<Character>(newCharacter);
            //character.Id = characters.Max(c => c.Id) + 1;
            //characters.Add(character);
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();
            //characters.Add(_mapper.Map<Character>(newCharacter));
            //serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            //var dbCharacters = await _context.Characters.ToListAsync();
            //serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            serviceResponse.Data = _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                //var character = characters.FirstOrDefault(c => c.Id == id);
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
                if (character is null)
                {
                  throw new Exception($"Character with ID: '{id}' not found!");
                }
               // _mapper.Map(uptCharacter,character);
                
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();

                serviceResponse.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();;
            }
            catch (System.Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;    
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await _context.Characters.ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
           //return characters.FirstOrDefault(c=> c.Id == id);
           //return characters.FirstOrDefault(c=> c.Id == id)!;
           var serviceResponse = new ServiceResponse<GetCharacterDto>();
           //var character = characters.FirstOrDefault(c => c.Id == id);
           var dbcharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
           //serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
           serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbcharacter);
           return serviceResponse;
           //if(character is not null)
           //     return character;

           //throw new Exception("Character not found!");
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto uptCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                //var character = characters.FirstOrDefault(c => c.Id == uptCharacter.Id);
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == uptCharacter.Id);
                if (character is null)
                {
                  throw new Exception($"Character with ID: '{uptCharacter.Id}' not found!");
                }
                _mapper.Map(uptCharacter,character);
                
                character.Name = uptCharacter.Name;
                character.HitPoints = uptCharacter.HitPoints;
                character.Streng = uptCharacter.Streng;
                character.Defence = uptCharacter.Defence;
                character.Intelligence = uptCharacter.Intelligence;
                character.Class = uptCharacter.Class;
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (System.Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;    
        }
    }
}